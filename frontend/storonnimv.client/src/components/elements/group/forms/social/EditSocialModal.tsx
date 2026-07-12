import {FC, useContext, useState} from "react";
import {ISocialNetwork} from "../../../../../models/group/IMemberInfo.ts";
import {Button, Form} from "react-bootstrap";
import {GlobalContext} from "../../../../contexts/shared/GlobalContext.tsx";

interface ISocialEditModalProps {
    item: ISocialNetwork;
}

const EditSocialModal: FC<ISocialEditModalProps> = ({item}) => {
    const {sendRequest, OnHideModal, serverRoute} = useContext(GlobalContext)!;

    const [name, setName] = useState<string>(item.socialNetwork);
    const [url, setUrl] = useState<string>(item.url);

    const handleSubmit = async () => {
        try {
            const formData = new FormData();
            formData.append("id", item.id.toString());
            formData.append("type", name);
            formData.append("url", url);

            const response = await sendRequest(
                `${serverRoute}/admin/socials`,
                "PATCH",
                formData,
                {"Content-Type": "application/json"}
            );

            if (response.status === 204) {
                alert('Соціальну мережу успішно змінено');
                OnHideModal();
            } else {
                alert('Соціальну мережу не змінено');
            }
        } catch (error) {
            console.error(error);
            alert('ПОМИЛКА ПРИ ЗМІНІ СОЦІАЛЬНОЇ МЕРЕЖІ');
        }
    };

    return (
        <div className="form-modal">
            <h2 className="form-modal__title">Редагувати соціальну мережу</h2>
            <Form
                className="form-modal__form"
                onSubmit={(e) => {
                e.preventDefault();
                handleSubmit();
            }}>
                <Form.Group className="form-modal__group">
                    <Form.Label
                        className="form-modal__label"
                    >
                        Назва соціальної мережі:
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

                <Button
                    className='form-modal__button form-modal__button--confirm'
                    type="submit"
                >
                    Змінити
                </Button>
                <Button
                    className="form-modal__button form-modal__button--cancel"
                    onClick={OnHideModal}
                >
                    Скасувати
                </Button>
            </Form>
        </div>
    );
};

export {EditSocialModal};