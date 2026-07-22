import {FC, useState, useContext} from "react";
import {GlobalContext} from "../../../contexts/shared/GlobalContext.tsx";
import {Container, Row, Col, Form, Button} from "react-bootstrap";
import {ModalLoading} from "../../shared/ModalLoading.tsx";
import {VideoType} from "../../../../models/video/IVideoModel.ts";

const AddVideoModal: FC = () => {
    const globalContext = useContext(GlobalContext);
    if (!globalContext) throw new Error("GlobalContext is not defined");

    const {sendRequest, OnHideModal, modalLoading, setModalLoading, serverRoute} = globalContext;

    const [title, setTitle] = useState("");
    const [videoFile, setVideoFile] = useState<File | null>(null);
    const [videoType, setVideoType] = useState<VideoType>("Performance");

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        setVideoFile(file || null);
    };

    const isFormValid = title.trim() !== "" && videoFile !== null;

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!isFormValid) return;

        setModalLoading(true);
        const formData = new FormData();
        formData.append("Url", videoFile!);
        formData.append("Title", title.trim());
        formData.append("Type", videoType);

        try {
            const response = await sendRequest(
                `${serverRoute}/admin/videos`,
                "POST",
                formData);
            if (response.status === 201) {
                alert(`Відео успішно додано!`);
                OnHideModal();
            } else {
                alert(`Помилка при додаванні відео`);
            }
        } catch (error) {
            alert(`Помилка при додаванні відео`);
            console.error(error);
        } finally {
            setModalLoading(false);
        }
    };

    if (modalLoading) return <ModalLoading/>;

    return (
        <Container className="form-modal">
            <Row>
                <Col xs={12}>
                    <h2 className="form-modal__title">Додати відео</h2>
                    <Form onSubmit={handleSubmit} className="form-modal__form">
                        <Form.Group controlId="add-video-title" className="form-modal__group">
                            <Form.Label className="form-modal__label">Заголовок:</Form.Label>
                            <Form.Control
                                type="text"
                                value={title}
                                onChange={(e) => setTitle(e.target.value)}
                                placeholder={`Введіть заголовок відео`}
                                required
                                className="form-modal__input"
                            />
                        </Form.Group>

                        <Form.Group controlId="add-video-type" className="form-modal__group">
                            <Form.Label className="form-modal__label">Тип відео:</Form.Label>
                            <Form.Select
                                value={videoType}
                                onChange={(e) => setVideoType(e.target.value as VideoType)}
                                className="form-modal__select"
                            >
                                <option value="Promotion">Promotion</option>
                                <option value="Performance">Performance</option>
                                <option value="Backstage">Backstage</option>
                                <option value="Repetition">Repetition</option>
                            </Form.Select>
                        </Form.Group>

                        <Form.Group controlId="add-video-file" className="form-modal__group">
                            <Form.Label className="form-modal__label">Завантажте відео:</Form.Label>
                            <Form.Control
                                type="file"
                                accept="video/mp4,.mp4"
                                onChange={handleFileChange}
                                className="form-modal__input"
                            />
                        </Form.Group>

                        <Button
                            variant="primary"
                            type="submit"
                            className="form-modal__button form-modal__button--confirm"
                            disabled={!isFormValid}
                        >
                            Додати відео
                        </Button>
                        <Button
                            variant="primary"
                            type="button"
                            className="form-modal__button form-modal__button--cancel"
                            onClick={OnHideModal}
                        >
                            Скасувати
                        </Button>
                    </Form>
                </Col>
            </Row>
        </Container>
    );
};

export {AddVideoModal};
