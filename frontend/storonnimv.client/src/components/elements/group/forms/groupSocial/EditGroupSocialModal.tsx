import {IGroupSocial} from "../../../../../models/groupSocials/IGroupSocial.ts";
import {FC, useContext, useState} from "react";
import {GlobalContext} from "../../../../contexts/shared/GlobalContext.tsx";
import {Button, Form} from "react-bootstrap";

interface IEditGroupSocialModalProps {
    item: IGroupSocial;
}

const EditGroupSocialModal: FC<IEditGroupSocialModalProps> = ({item}) => {
    const {OnHideModal, sendRequest, serverRoute} = useContext(GlobalContext)!;

    const [linkUrl, setLinkUrl] = useState<string>(item.linkUrl);

    const handleSubmit = async () => {
        const data = {
            id: item.id,
            linkUrl
        };

        try {
            const response = await sendRequest(
                `${serverRoute}/admin/group-socials`,
                "PATCH",
                data,
                {"Content-Type": "application/json"}
            );

            if (response.status === 204) {
                alert("Дані успішно змінено!");
                OnHideModal();
                window.location.reload();
            } else {
                alert("Помилка при зміні даних");
            }
        } catch (error) {
            console.error("Помилка при зміні даних", error);
            alert("Помилка при зміні даних");
        }
    };

    return (
        <div className='form-modal'>
            <h2 className='form-modal__title'>Редагувати соціальну мережу групи</h2>

            <Form
                className='form-modal__form'
                onSubmit={(e) => {
                    e.preventDefault();
                    handleSubmit();
                }}
            >
                <Form.Group className='form-modal__group'>
                    <Form.Label className="form-modal__label">Посилання:</Form.Label>
                    <Form.Control
                        type="text"
                        value={linkUrl}
                        required
                        onChange={(e) => setLinkUrl(e.target.value)}
                        className="form-modal__input"
                    />
                </Form.Group>

                <Button type="submit" className="form-modal__button form-modal__button--confirm">
                    Зберегти зміни
                </Button>
            </Form>
        </div>
    );
};

export {EditGroupSocialModal};
