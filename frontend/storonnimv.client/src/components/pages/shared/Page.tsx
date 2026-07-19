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
import performanceImage from "../../../assets/video-categories/performance.webp";
import backstageImage from "../../../assets/video-categories/backstage.webp";
import repetitionImage from "../../../assets/video-categories/repetition.webp";

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
                                topImage={performanceImage}
                                bottomLeftImage={backstageImage}
                                bottomRightImage={repetitionImage}
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
