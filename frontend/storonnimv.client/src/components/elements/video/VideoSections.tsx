import { FC, useContext } from "react";
import { useNavigate } from "react-router-dom";
import { Button } from "react-bootstrap";
import { AddVideoModal } from "./forms/AddVideoModal.tsx";
import { FaPlus } from "react-icons/fa";
import { GlobalContext } from "../../contexts/shared/GlobalContext.tsx";

interface VideoSectionsProps {
    topImage?: string;
    bottomLeftImage?: string;
    bottomRightImage?: string;
}

const VideoSections: FC<VideoSectionsProps> = ({ topImage, bottomLeftImage, bottomRightImage }) => {
    const globalContext = useContext(GlobalContext)!;
    const { isAdmin, OnShowModal } = globalContext;
    const navigate = useNavigate();

    const handleNavigate = (videoType: string) => {
        navigate(`/video/section?videoType=${videoType}`);
    };

    const isMainPage = window.location.pathname === "/video/sections";

    return (
        <div className="video-sections__container">
            {isAdmin && isMainPage && (
                <Button
                    className="admin-button__add"
                    onClick={() => OnShowModal(<AddVideoModal />)}
                >
                    <FaPlus />
                </Button>
            )}

            <div className="video-sections__grid">
                <Button
                    variant="light"
                    className="video-sections__button"
                    onClick={() => handleNavigate("Performance")}
                >
                    {topImage ? (
                        <img src={topImage} alt="Top Section" className="video-sections__image" />
                    ) : (
                        <p className="video-sections__placeholder">Top Section (Add an image)</p>
                    )}
                    <h1 className="video-sections__title main-text">Виступи</h1>
                </Button>

                <Button
                    variant="light"
                    className="video-sections__button"
                    onClick={() => handleNavigate("Backstage")}
                >
                    {bottomLeftImage ? (
                        <img src={bottomLeftImage} alt="Bottom Left Section" className="video-sections__image" />
                    ) : (
                        <p className="video-sections__placeholder">Bottom Left Section (Add an image)</p>
                    )}
                    <h1 className="video-sections__title main-text">За лаштунками</h1>
                </Button>

                <Button
                    variant="light"
                    className="video-sections__button"
                    onClick={() => handleNavigate("Repetition")}
                >
                    {bottomRightImage ? (
                        <img src={bottomRightImage} alt="Bottom Right Section" className="video-sections__image" />
                    ) : (
                        <p className="video-sections__placeholder">Bottom Right Section (Add an image)</p>
                    )}
                    <h1 className="video-sections__title main-text">Репетиції</h1>
                </Button>
            </div>
        </div>
    );
};

export { VideoSections };
