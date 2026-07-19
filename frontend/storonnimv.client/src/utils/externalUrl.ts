const getSafeExternalUrl = (value: string): string | undefined => {
    if (value !== value.trim()) return undefined;

    try {
        const url = new URL(value);
        const usesHttp = url.protocol === "http:" || url.protocol === "https:";
        const hasCredentials = url.username !== "" || url.password !== "";

        return usesHttp && !hasCredentials ? url.href : undefined;
    } catch {
        return undefined;
    }
};

export {getSafeExternalUrl};
