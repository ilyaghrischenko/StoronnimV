import React, { useContext } from "react";
import { Button } from "react-bootstrap";
import { IVideoModel } from "../../../models/video/IVideoModel";
import { GlobalContext } from "../../contexts/shared/GlobalContext";
import { EditVideoModal } from "./forms/EditVideoModal.tsx";
import { DeleteVideoModal } from "./forms/DeleteVideoModal.tsx";
import {FaEdit, FaTrash} from "react-icons/fa";

interface IVideoListItemProps {
    videoItem: IVideoModel;
}

const VideoListItem: React.FC<IVideoListItemProps> = ({ videoItem }) => {
    const { OnShowModal, isAdmin } = useContext(GlobalContext)!;

    return (
        <div className="video-list-item">
            <h2
                className="video-list-item__title main-text"
                title={videoItem.title}
            >{videoItem.title}</h2>
            <video
                className="video-list-item__video"
                controls
                preload="metadata"
                playsInline
                aria-label={`Відео: ${videoItem.title}`}
            >
                <source src={videoItem.url} type="video/mp4" />
                Ваш браузер не підтримує тег video.
            </video>
            {isAdmin && (
                <div className="video-list-item__admin-buttons admin-controls">
                    <p className="video-list-item__admin-id">Video id: {videoItem.id}</p>
                    <Button
                        className="admin-control"
                        type="button"
                        aria-label={`Редагувати відео ${videoItem.title}`}
                        onClick={() => OnShowModal(<EditVideoModal video={videoItem}/>)}
                    >
                        <FaEdit/>
                    </Button>
                    <Button
                        className="admin-control"
                        type="button"
                        aria-label={`Видалити відео ${videoItem.title}`}
                        onClick={() => OnShowModal(<DeleteVideoModal video={videoItem} />)}
                    >
                        <FaTrash/>
                    </Button>
                </div>
            )}
        </div>
    );
};

export { VideoListItem };
