import React, {useContext, useState} from "react";
import {GlobalContext} from "../../../contexts/shared/GlobalContext.tsx";
import {Form, Button, Container} from "react-bootstrap";
import {ModalLoading} from "../../shared/ModalLoading.tsx";

const AddNewsItemModal: React.FC = () => {
    const globalContext = useContext(GlobalContext)!;

    const {sendRequest, OnHideModal, setModalLoading, modalLoading, serverRoute} = globalContext;

    const [formData, setFormData] = useState({
        title: "",
        description: "",
        priority: "Secondary",
        date: "",
    });

    const [photo, setPhoto] = useState<File | null>(null);
    const [video, setVideo] = useState<string>("");

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
        setFormData({...formData, [e.target.name]: e.target.value});
    };

    const handleVideoChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
        setVideo(e.target.value);
    };

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>, setFile: React.Dispatch<React.SetStateAction<File | null>>, fileType: string) => {
        const file = e.target.files?.[0];
        if (file && file.type.startsWith(fileType)) {
            setFile(file);
        } else {
            alert(`Будь ласка, завантажте файл типу ${fileType}.`);
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setModalLoading(true);

        const data = new FormData();
        Object.entries(formData).forEach(([key, value]) => data.append(key, value));
        if (photo) data.append("photo", photo);
        if (video !== "" && /^\d+$/.test(video)) data.append("videoId", video);

        try {
            const response = await sendRequest(`${serverRoute}/admin/news`, "POST", data);
            if (response.status === 201) {
                window.location.reload();
                OnHideModal();
            } else {
                alert("Помилка при додаванні новини" + response.status);
                console.error(response.status);
                OnHideModal();
            }
        } catch (error) {
            alert(`Помилка при додаванні новини ${error}`);
        } finally {
            setModalLoading(false);
        }
    };

    if (modalLoading) return <ModalLoading/>;

    return (
        <Container className="form-modal">
            <h2 className="form-modal__title">Додати новину</h2>
            <Form className="form-modal__form" onSubmit={handleSubmit}>
                <Form.Group controlId="add-news-title" className="form-modal__group">
                    <Form.Label className="form-modal__label">Заголовок:</Form.Label>
                    <Form.Control className="form-modal__input" type="text" name="title" value={formData.title}
                                  onChange={handleInputChange} required/>
                </Form.Group>

                <Form.Group controlId="add-news-description" className="form-modal__group">
                    <Form.Label className="form-modal__label">Опис:</Form.Label>
                    <Form.Control className="form-modal__input" as="textarea" rows={3} name="description"
                                  value={formData.description} onChange={handleInputChange} required/>
                </Form.Group>

                <Form.Group controlId="add-news-priority" className="form-modal__group">
                    <Form.Label className="form-modal__label">Пріоритет:</Form.Label>
                    <Form.Select className="form-modal__select" name="priority" value={formData.priority}
                                 onChange={handleInputChange}>
                        <option value="Secondary">Secondary</option>
                        <option value="Main">Main</option>
                    </Form.Select>
                </Form.Group>

                <Form.Group controlId="add-news-date" className="form-modal__group">
                    <Form.Label className="form-modal__label">Дата:</Form.Label>
                    <Form.Control className="form-modal__input" type="date" name="date" value={formData.date}
                                  onChange={handleInputChange} required/>
                </Form.Group>

                <Form.Group controlId="add-news-photo" className="form-modal__group">
                    <Form.Label className="form-modal__label">Фото (необов'язково):</Form.Label>
                    <Form.Control type="file" accept=".jpg,.jpeg,.png,.webp,image/jpeg,image/png,image/webp" className="form-modal__input"
                                  onChange={(e: React.ChangeEvent<HTMLInputElement>) => handleFileChange(e, setPhoto, "image/")}/>
                </Form.Group>

                <Form.Group controlId="add-news-video-id" className="form-modal__group">
                    <Form.Label className="form-modal__label">Відео (необов'язково, для додавання ввести його
                        id):</Form.Label>
                    <Form.Control type="text" className="form-modal__input" value={video}
                                  onChange={handleVideoChange}/>
                </Form.Group>

                <Button className="form-modal__button form-modal__button--confirm" type="submit">Додати новину</Button>
                <Button className="form-modal__button form-modal__button--cancel" type="button" onClick={OnHideModal}>Скасувати</Button>
            </Form>
        </Container>
    );
};

export {AddNewsItemModal};
