import {FC, useContext, useEffect} from "react";
import {GlobalContext} from "../../contexts/shared/GlobalContext";
import {List} from "../shared/GenericList/List";
import {ListItem} from "../shared/GenericList/ListItem";
import {VideoContext} from "../../contexts/VideoContext";
import {Navigate, useNavigate, useSearchParams} from "react-router-dom";
import {isVideoCategory, IVideoModel} from "../../../models/video/IVideoModel";
import {VideoListItem} from "./VideoListItem";
import {PaginationSection} from "../shared/PaginationSection.tsx";
import PreloaderTile from "../shared/PreloaderTile.tsx";
import {NoData} from "../shared/NoData.tsx";

const VideoList: FC = () => {
    const [searchParams] = useSearchParams();
    const videoTypeParameter = searchParams.get("videoType") ?? "Performance";
    const videoType = isVideoCategory(videoTypeParameter) ? videoTypeParameter : null;

    const navigate = useNavigate();

    const videoContext = useContext(VideoContext);
    const globalContext = useContext(GlobalContext);

    if (!globalContext) {
        throw new Error("GlobalContext must be used within a GlobalContextProvider");
    }
    if (!videoContext) {
        throw new Error("VideoContext must be used within a VideoContextProvider");
    }

    const {pageLoading} = globalContext;
    const {videoList, currentPage, totalPages, videoListStatus, paginate} = videoContext;

    useEffect(() => {
        if (videoType) {
            void paginate(videoType, 1, 2);
        }
    }, [paginate, videoType]);

    if (!videoType) {
        return <Navigate to="/error?statusCode=404&message=Video%20type%20not%20found" replace/>;
    }

    const onBackButtonClick = () => {
        navigate('/video/sections');
    };

    return (
        <div className="video-list-container">
            <button
                className="video-btn"
                onClick={onBackButtonClick}
            >
                <span className="icon">&#x276E;</span>
                <span className="label small-shadow">НАЗАД</span>
            </button>

            {pageLoading || videoListStatus === "idle" || videoListStatus === "loading" ?
                <List
                    className="video-list"
                    items={[0, 1]}
                    renderItem={(item: number) => (
                        <ListItem
                            key={item}
                            item={item}
                            renderItem={() => <PreloaderTile className='preloader-tile__container-video-page'/>}
                        />
                    )}
                /> : videoListStatus === "error" ?
                <NoData
                    variant="error"
                    message='Не вдалося завантажити відео'
                    actionLabel='Спробувати ще раз'
                    onAction={() => paginate(videoType, currentPage, 2)}
                /> : videoListStatus === "empty" ?
                <NoData message='Відео немає'/> :
                <List
                    className="video-list"
                    items={videoList}
                    renderItem={(item: IVideoModel) => (
                        <ListItem
                            key={item.id}
                            item={item}
                            renderItem={(item: IVideoModel) => (
                                <VideoListItem videoItem={item}/>
                            )}
                        />
                    )}
                />
            }

            {videoListStatus === "success" && totalPages > 1 &&
                <PaginationSection
                    currentPage={currentPage}
                    totalPages={totalPages}
                    paginate={(page) => paginate(videoType, page, 2)}
                    compactOnMobile
                />}
        </div>
    );
};

export {VideoList};
