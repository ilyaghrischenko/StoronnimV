import {FC, useContext, useState} from "react";
import {Button, Form} from "react-bootstrap";
import {GlobalContext} from "../../../../contexts/shared/GlobalContext.tsx";

interface ISocialAddModalProps {
    memberId: number;
}

const AddSocialModal: FC<ISocialAddModalProps> = ({memberId}) => {
    const {sendRequest, OnHideModal, serverRoute} = useContext(GlobalContext)!;

    const [name, setName] = useState<string>("");
    const [url, setUrl] = useState<string>("");

    const handleSubmit = async () => {
        const formData = new FormData();
        formData.append("memberId", memberId.toString());
        formData.append("url", url);
        formData.append("type", name);

        try {
            const response = await sendRequest(
                `${serverRoute}/admin/socials`,
                'POST',
                formData,
                {"Content-Type": "application/json"}
            );

            if (response.status === 201) {
                alert('Соціальну мережу успішно додано');
                OnHideModal();
            } else {
                alert('Соціальну мережу не додано');
            }
        } catch (error) {
            console.error(error);
            alert('Помилка при додаванні соціальної мережі');
        }
    };

    return (
        <div className="form-modal">
            <h2 className='form-modal__title'>Додати соціальну мережу</h2>
            <Form
                className='form-modal__form'
                onSubmit={(e) => {
                    e.preventDefault();
                    handleSubmit();
                }}>
                <Form.Group className='form-modal__group'>
                    <Form.Label
                        className="form-modal__label">
                        Назва:
                    </Form.Label>
                    <Form.Control
                        type="text"
                        className="form-modal__input"
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                    />
                </Form.Group>
                <Form.Group className="form-modal__group">
                    <Form.Label
                        className="form-modal__label"
                    >
                        Посилання:
                    </Form.Label>
                    <Form.Control
                        type="text"
                        className="form-modal__input"
                        value={url}
                        onChange={(e) => setUrl(e.target.value)}
                    />
                </Form.Group>

                <Button className="form-modal__button form-modal__button--confirm" type="submit">
                    Додати
                </Button>
                <Button className="form-modal__button form-modal__button--cancel" onClick={OnHideModal}>
                    Скасувати
                </Button>
            </Form>
        </div>
    );
};

export {AddSocialModal};