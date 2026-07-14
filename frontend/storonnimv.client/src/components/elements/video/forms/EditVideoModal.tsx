import React, {FC, useContext, useState, useEffect} from "react";
import {Button, Container, Form} from "react-bootstrap";
import {GlobalContext} from "../../../contexts/shared/GlobalContext.tsx";
import {IVideoModel} from "../../../../models/video/IVideoModel.tsx";

interface VideoEditButtonProps {
    video: IVideoModel;
}

const EditVideoModal: FC<VideoEditButtonProps> = ({video}) => {
    const globalContext = useContext(GlobalContext)!;
    const [editedVideo, setEditedVideo] = useState<IVideoModel>(video);

    const {sendRequest, OnHideModal, serverRoute} = globalContext;

    useEffect(() => {
        setEditedVideo(video);
    }, [video]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
        setEditedVideo({
            ...editedVideo,
            [e.target.name]: e.target.value,
        });
    };


    const handleSave = async (event: React.FormEvent) => {
        event.preventDefault();

        try {
            const data = {
                id: editedVideo.id,
                title: editedVideo.title,
                type: editedVideo.type
            };

            const response = await sendRequest(
                `${serverRoute}/admin/videos`,
                "PATCH",
                data,
                {"Content-Type": "application/json"}
            );

            if (response.status === 204) {
                OnHideModal();
                alert("Збережено!");
                window.location.reload();
            } else {
                throw new Error("Помилка при збереженні відео: " + response.status);
            }
        } catch (error) {
            console.error("Помилка при збереженні відео:", error);
        }
    };

    return (
        <Container className="form-modal">
            <h2 className="form-modal__title">Редагувати відео</h2>
            <Form className="form-modal__form"
                  onSubmit={handleSave}
            >
                <Form.Group className="form-modal__group">
                    <Form.Label className="form-modal__label">Заголовок:</Form.Label>
                    <Form.Control
                        type="text"
                        name="title"
                        value={editedVideo.title || ""}
                        onChange={handleChange}
                        className="form-modal__input"
                        placeholder="Введіть назву відео"
                    />
                </Form.Group>
                <Form.Group className="form-modal__group">
                    <Form.Label className="form-modal__label">Змінити тип відео:</Form.Label>
                    <Form.Select
                        name="type"
                        value={editedVideo.type || ""}
                        onChange={handleChange}
                        className="form-modal__select"
                    >
                        <option value="Performance">Performance</option>
                        <option value="Backstage">Backstage</option>
                        <option value="Repetition">Repetition</option>
                    </Form.Select>
                </Form.Group>
                <Button className="form-modal__button form-modal__button--confirm" type='submit'>
                    Зберегти
                </Button>
                <Button className="form-modal__button form-modal__button--cancel" onClick={OnHideModal}>
                    Закрити
                </Button>
            </Form>
        </Container>
    );
};

export {EditVideoModal};
