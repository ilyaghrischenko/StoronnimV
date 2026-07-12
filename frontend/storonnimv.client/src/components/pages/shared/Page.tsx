import { FC } from "react";
import { Navigate, Route, Routes } from "react-router-dom";
import { Home } from "../Home";
import { Schedule } from "../Schedule";
import { News } from "../News";
import { Music } from "../Music";
import { Group } from "../Group";
import { Video } from "../Video";
import { VideoSections } from "../../elements/video/VideoSections";
import { VideoList } from "../../elements/video/VideoList";
import { Admin } from "../Admin.tsx";
import { AuthForm } from "../../elements/admin/AuthForm.tsx";
import { Error } from "../Error.tsx";
import { ProtectedRoute } from "../../elements/admin/ProtectedRoute.tsx";
import { AdminContainer } from "../../elements/admin/AdminContainer.tsx";
import {Developers} from "../Developers.tsx";

const Page: FC = () => {
    return (
        <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/schedule" element={<Schedule />} />
            <Route path="/news" element={<News />} />
            <Route path="/music" element={<Music />} />
            <Route path="/group" element={<Group />} />
            <Route
                path="/video/sections"
                element={
                    <Video
                        children={
                            <VideoSections
                                topImage={
                                    "https://th.bing.com/th/id/OIP.sl5zuf2713AebuRLfZOJeAHaE6?rs=1&pid=ImgDetMain"
                                }
                                bottomLeftImage={
                                    "https://th.bing.com/th/id/OIP.sl5zuf2713AebuRLfZOJeAHaE6?rs=1&pid=ImgDetMain"
                                }
                                bottomRightImage={
                                    "https://th.bing.com/th/id/OIP.sl5zuf2713AebuRLfZOJeAHaE6?rs=1&pid=ImgDetMain"
                                }
                            />
                        }
                    />
                }
            />
            <Route path="/video/section" element={<Video children={<VideoList />} />} />
            <Route path="/admin" element={<Admin children={<AuthForm />} />} />

            <Route
                path="/admin/basic-admins"
                element={
                    <ProtectedRoute requiredRole="SuperAdmin">
                        <Admin>
                            <AdminContainer />
                        </Admin>
                    </ProtectedRoute>
                }
            />

            <Route path='/developers' element={<Developers />} />

            <Route path="/error" element={<Error />} />
            <Route path="*" element={<Navigate to="/error?statusCode=404&message=Not%20Found" />} />
        </Routes>
    );
};

export { Page };