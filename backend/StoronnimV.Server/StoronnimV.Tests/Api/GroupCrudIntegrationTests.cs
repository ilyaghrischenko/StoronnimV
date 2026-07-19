using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using StoronnimV.Application.DTO.Responses.GroupPage;
using StoronnimV.Domain.Entities;
using StoronnimV.Infrastructure;
using Xunit.Sdk;

namespace StoronnimV.Tests.Api;

public sealed class GroupCrudIntegrationTests(AuthApiFactory factory)
    : IClassFixture<AuthApiFactory>
{
    [Fact]
    public async Task GroupPageDatabase_RejectsSecondRow()
    {
        (string dbConnection, _) = RequireIntegrationEnvironment();
        string marker = $"feat06-db-{Guid.NewGuid():N}";

        try
        {
            await using StoronnimVContext context = CreateContext(dbConnection);
            context.GroupPages.Add(new GroupPage
            {
                Description = $"{marker}-first",
                PhotoUrl = "https://example.test/first.jpg"
            });
            await context.SaveChangesAsync();

            context.GroupPages.Add(new GroupPage
            {
                Description = $"{marker}-second",
                PhotoUrl = "https://example.test/second.jpg"
            });

            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }
        finally
        {
            await using StoronnimVContext cleanupContext = CreateContext(dbConnection);
            await cleanupContext.GroupPages
                .Where(x => x.Description.StartsWith(marker))
                .ExecuteDeleteAsync();
        }
    }

    [Fact]
    public async Task GroupCrud_RealApiPostgresAndAzurite_EnforcesSingletonAndPersistsMemberSocialReadback()
    {
        (string dbConnection, string blobConnection) = RequireIntegrationEnvironment();
        string marker = $"feat06-{Guid.NewGuid():N}";
        List<string> blobUrls = [];

        try
        {
            BlobContainerClient photoContainer = new BlobServiceClient(blobConnection)
                .GetBlobContainerClient("storonnimv-photo");
            await photoContainer.CreateIfNotExistsAsync();
            await photoContainer.SetAccessPolicyAsync(PublicAccessType.Blob);

            using HttpClient client = factory.CreateApiClient();
            string token = factory.CreateToken("Basic");

            using (MultipartFormDataContent createGroupContent = new()
            {
                { new StringContent($"{marker}-group-created"), "description" },
                { CreateFileContent([0xFF, 0xD8, 0xFF, 0xE0], "image/jpeg"), "photoUrl", "group-created.jpg" }
            })
            using (HttpRequestMessage createGroupRequest = Authenticated(
                       HttpMethod.Post, "/api/admin/group", token, createGroupContent))
            {
                Assert.Equal(HttpStatusCode.Created, (await client.SendAsync(createGroupRequest)).StatusCode);
            }

            long groupId;
            await using (StoronnimVContext context = CreateContext(dbConnection))
            {
                GroupPage group = await context.GroupPages.SingleAsync(x => x.Description == $"{marker}-group-created");
                groupId = group.Id;
                blobUrls.Add(group.PhotoUrl);
            }

            GroupPageFullInfoResponse createdGroup = await GetGroupAsync(client);
            Assert.Equal(groupId, createdGroup.GroupPage.Id);
            Assert.Equal($"{marker}-group-created", createdGroup.GroupPage.Description);
            Assert.Equal(blobUrls[0], createdGroup.GroupPage.PhotoUrl);
            Assert.Empty(createdGroup.Members);
            Assert.True(await BlobExistsAsync(blobConnection, blobUrls[0]));

            using (MultipartFormDataContent duplicateContent = new()
            {
                { new StringContent($"{marker}-group-duplicate"), "description" },
                { CreateFileContent([0xFF, 0xD8, 0xFF, 0xE0], "image/jpeg"), "photoUrl", "group-duplicate.jpg" }
            })
            using (HttpRequestMessage duplicateRequest = Authenticated(
                       HttpMethod.Post, "/api/admin/group", token, duplicateContent))
            {
                Assert.Equal(HttpStatusCode.BadRequest, (await client.SendAsync(duplicateRequest)).StatusCode);
            }

            await using (StoronnimVContext context = CreateContext(dbConnection))
            {
                Assert.Equal(1, await context.GroupPages.CountAsync());
            }

            using (HttpRequestMessage editGroupRequest = Authenticated(
                       HttpMethod.Patch,
                       "/api/admin/group-pages",
                       token,
                       JsonContent.Create(new { id = groupId, description = $"{marker}-group-edited" })))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(editGroupRequest)).StatusCode);
            }
            Assert.Equal($"{marker}-group-edited", (await GetGroupAsync(client)).GroupPage.Description);

            using (MultipartFormDataContent groupPhotoContent = new()
            {
                { new StringContent(groupId.ToString()), "id" },
                { CreateFileContent([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A], "image/png"), "photo", "group-edited.png" }
            })
            using (HttpRequestMessage groupPhotoRequest = Authenticated(
                       HttpMethod.Patch, "/api/admin/group-page/photo", token, groupPhotoContent))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(groupPhotoRequest)).StatusCode);
            }

            string editedGroupPhoto = (await GetGroupAsync(client)).GroupPage.PhotoUrl;
            blobUrls.Add(editedGroupPhoto);
            Assert.NotEqual(blobUrls[0], editedGroupPhoto);
            Assert.False(await BlobExistsAsync(blobConnection, blobUrls[0]));
            Assert.True(await BlobExistsAsync(blobConnection, editedGroupPhoto));

            using (MultipartFormDataContent createMemberContent = new()
            {
                { new StringContent($"{marker}-member-created"), "fullName" },
                { new StringContent("Created member description"), "description" },
                { new StringContent("Guitar"), "role" },
                { CreateFileContent([0xFF, 0xD8, 0xFF, 0xE0], "image/jpeg"), "photoUrl", "member-created.jpg" }
            })
            using (HttpRequestMessage createMemberRequest = Authenticated(
                       HttpMethod.Post, "/api/admin/group/members", token, createMemberContent))
            {
                Assert.Equal(HttpStatusCode.Created, (await client.SendAsync(createMemberRequest)).StatusCode);
            }

            long memberId;
            await using (StoronnimVContext context = CreateContext(dbConnection))
            {
                Member member = await context.Members.SingleAsync(x => x.FullName == $"{marker}-member-created");
                memberId = member.Id;
                blobUrls.Add(member.PhotoUrl);
            }

            GroupPageFullInfoResponse groupWithMember = await GetGroupAsync(client);
            Assert.Contains(groupWithMember.Members, member =>
                member.Id == memberId &&
                member.FullName == $"{marker}-member-created" &&
                member.Role == "Guitar" &&
                member.PhotoUrl == blobUrls[2]);

            MemberFullInfoResponse createdMember = await GetMemberAsync(client, memberId);
            Assert.Equal("Created member description", createdMember.Description);
            Assert.Empty(createdMember.Socials);

            using (HttpRequestMessage editMemberRequest = Authenticated(
                       HttpMethod.Patch,
                       "/api/admin/group-pages/members",
                       token,
                       JsonContent.Create(new
                       {
                           id = memberId,
                           fullName = $"{marker}-member-edited",
                           description = "Edited member description",
                           role = "Lead guitar"
                       })))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(editMemberRequest)).StatusCode);
            }

            MemberFullInfoResponse editedMember = await GetMemberAsync(client, memberId);
            Assert.Equal($"{marker}-member-edited", editedMember.FullName);
            Assert.Equal("Edited member description", editedMember.Description);
            Assert.Equal("Lead guitar", editedMember.Role);

            using (MultipartFormDataContent memberPhotoContent = new()
            {
                { new StringContent(memberId.ToString()), "id" },
                { CreateFileContent([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A], "image/png"), "photo", "member-edited.png" }
            })
            using (HttpRequestMessage memberPhotoRequest = Authenticated(
                       HttpMethod.Patch, "/api/admin/group-page/members/photo", token, memberPhotoContent))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(memberPhotoRequest)).StatusCode);
            }

            string editedMemberPhoto = (await GetMemberAsync(client, memberId)).PhotoUrl;
            blobUrls.Add(editedMemberPhoto);
            Assert.NotEqual(blobUrls[2], editedMemberPhoto);
            Assert.False(await BlobExistsAsync(blobConnection, blobUrls[2]));
            Assert.True(await BlobExistsAsync(blobConnection, editedMemberPhoto));

            using (HttpRequestMessage createSocialRequest = Authenticated(
                       HttpMethod.Post,
                       "/api/admin/socials",
                       token,
                       JsonContent.Create(new
                       {
                           memberId,
                           url = "https://instagram.example/created",
                           type = "Instagram"
                       })))
            {
                Assert.Equal(HttpStatusCode.Created, (await client.SendAsync(createSocialRequest)).StatusCode);
            }

            MemberFullInfoResponse memberWithSocial = await GetMemberAsync(client, memberId);
            var createdSocial = Assert.Single(memberWithSocial.Socials);
            Assert.Equal("Instagram", createdSocial.SocialNetwork);
            Assert.Equal("https://instagram.example/created", createdSocial.Url);

            using (HttpRequestMessage editSocialRequest = Authenticated(
                       HttpMethod.Patch,
                       "/api/admin/socials",
                       token,
                       JsonContent.Create(new
                       {
                           id = createdSocial.Id,
                           url = "https://telegram.example/edited",
                           type = "Telegram"
                       })))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(editSocialRequest)).StatusCode);
            }

            var editedSocial = Assert.Single((await GetMemberAsync(client, memberId)).Socials);
            Assert.Equal("Telegram", editedSocial.SocialNetwork);
            Assert.Equal("https://telegram.example/edited", editedSocial.Url);

            using (HttpRequestMessage deleteSocialRequest = Authenticated(
                       HttpMethod.Delete, $"/api/admin/socials/{createdSocial.Id}", token))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(deleteSocialRequest)).StatusCode);
            }
            Assert.Empty((await GetMemberAsync(client, memberId)).Socials);

            using (HttpRequestMessage deleteMemberRequest = Authenticated(
                       HttpMethod.Delete, $"/api/admin/group/members/{memberId}", token))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(deleteMemberRequest)).StatusCode);
            }
            Assert.DoesNotContain((await GetGroupAsync(client)).Members, member => member.Id == memberId);
            Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/group/member/{memberId}")).StatusCode);
            Assert.False(await BlobExistsAsync(blobConnection, editedMemberPhoto));

            using (HttpRequestMessage deleteGroupRequest = Authenticated(
                       HttpMethod.Delete, $"/api/admin/group/{groupId}", token))
            {
                Assert.Equal(HttpStatusCode.NoContent, (await client.SendAsync(deleteGroupRequest)).StatusCode);
            }
            Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/group")).StatusCode);
            Assert.False(await BlobExistsAsync(blobConnection, editedGroupPhoto));
        }
        finally
        {
            await CleanupAsync(dbConnection, blobConnection, marker, blobUrls);
        }
    }

    private static (string DbConnection, string BlobConnection) RequireIntegrationEnvironment()
    {
        if (Environment.GetEnvironmentVariable("FEAT06_INTEGRATION") != "1")
        {
            throw SkipException.ForSkip(
                "Set FEAT06_INTEGRATION=1 with disposable DB_CLOUD and BLOB_STORAGE targets.");
        }

        string dbConnection = Environment.GetEnvironmentVariable("DB_CLOUD")
            ?? throw new InvalidOperationException("DB_CLOUD is required for FEAT-06 integration tests.");
        string blobConnection = Environment.GetEnvironmentVariable("BLOB_STORAGE")
            ?? throw new InvalidOperationException("BLOB_STORAGE is required for FEAT-06 integration tests.");
        return (dbConnection, blobConnection);
    }

    private static StoronnimVContext CreateContext(string connectionString)
    {
        DbContextOptions<StoronnimVContext> options = new DbContextOptionsBuilder<StoronnimVContext>()
            .UseNpgsql(connectionString)
            .Options;
        return new StoronnimVContext(options);
    }

    private static ByteArrayContent CreateFileContent(byte[] bytes, string contentType)
    {
        ByteArrayContent content = new(bytes);
        content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        return content;
    }

    private static HttpRequestMessage Authenticated(
        HttpMethod method,
        string route,
        string token,
        HttpContent? content = null)
    {
        HttpRequestMessage request = new(method, route) { Content = content };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    private static async Task<GroupPageFullInfoResponse> GetGroupAsync(HttpClient client)
    {
        HttpResponseMessage response = await client.GetAsync("/api/group");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return Assert.IsType<GroupPageFullInfoResponse>(
            await response.Content.ReadFromJsonAsync<GroupPageFullInfoResponse>());
    }

    private static async Task<MemberFullInfoResponse> GetMemberAsync(HttpClient client, long id)
    {
        HttpResponseMessage response = await client.GetAsync($"/api/group/member/{id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return Assert.IsType<MemberFullInfoResponse>(
            await response.Content.ReadFromJsonAsync<MemberFullInfoResponse>());
    }

    private static async Task<bool> BlobExistsAsync(string connectionString, string url)
    {
        Uri uri = new(url);
        string containerName = uri.Segments[^2].Trim('/');
        string blobName = Uri.UnescapeDataString(uri.Segments[^1]);
        return await new BlobServiceClient(connectionString)
            .GetBlobContainerClient(containerName)
            .GetBlobClient(blobName)
            .ExistsAsync();
    }

    private static async Task CleanupAsync(
        string dbConnection,
        string blobConnection,
        string marker,
        List<string> blobUrls)
    {
        await using (StoronnimVContext context = CreateContext(dbConnection))
        {
            blobUrls.AddRange(await context.GroupPages
                .Where(x => x.Description.StartsWith(marker))
                .Select(x => x.PhotoUrl)
                .ToListAsync());
            blobUrls.AddRange(await context.Members
                .Where(x => x.FullName.StartsWith(marker))
                .Select(x => x.PhotoUrl)
                .ToListAsync());

            await context.Socials
                .Where(x => x.Member.FullName.StartsWith(marker))
                .ExecuteDeleteAsync();
            await context.Members
                .Where(x => x.FullName.StartsWith(marker))
                .ExecuteDeleteAsync();
            await context.GroupPages
                .Where(x => x.Description.StartsWith(marker))
                .ExecuteDeleteAsync();
        }

        foreach (string url in blobUrls.Distinct())
        {
            Uri uri = new(url);
            string containerName = uri.Segments[^2].Trim('/');
            string blobName = Uri.UnescapeDataString(uri.Segments[^1]);
            await new BlobServiceClient(blobConnection)
                .GetBlobContainerClient(containerName)
                .DeleteBlobIfExistsAsync(blobName);
        }
    }
}
