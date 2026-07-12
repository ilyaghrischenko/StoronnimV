import { FC, useContext } from "react";
import { Button, Container } from "react-bootstrap";
import { GlobalContext } from "../../../contexts/shared/GlobalContext.tsx";
import { IVideoModel } from "../../../../models/video/IVideoModel.ts";

interface DeleteVideoModalProps {
    video: IVideoModel;
}

const DeleteVideoModal: FC<DeleteVideoModalProps> = ({ video}) => {
    const globalContext = useContext(GlobalContext)!;

    const { OnHideModal, sendRequest, serverRoute } = globalContext;

    const handleDelete = async () => {
        try {
            const response = await sendRequest(`${serverRoute}/admin/videos/${video.id}`, "DELETE");

            if (response.status === 204) {
                console.log("Відео успішно видалено:", video);
                alert("Відео успішно видалено!");
                OnHideModal();
                window.location.reload();
            } else {
                alert("Помилка при видаленні відео.");
            }
        } catch (error) {
            console.error("Помилка при видаленні відео:", error);
            alert("Сталася помилка при видаленні відео.");
        }
    };

    return (
        <Container className="form-modal">
            <h2 className="form-modal__title">
                Ви впевнені, що хочете видалити це відео?
            </h2>
            <Container className="form-modal__form">
                <Button
                    variant="danger"
                    onClick={handleDelete}
                    className="form-modal__button form-modal__button--delete"
                >
                    Так, видалити
                </Button>
                <Button
                    variant="secondary"
                    onClick={OnHideModal}
                    className="form-modal__button form-modal__button--cancel"
                >
                    Скасувати
                </Button>
            </Container>
        </Container>
    );
};

export { DeleteVideoModal };
