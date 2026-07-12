import {FC, useContext, useEffect, useState} from "react";
import {GlobalContext} from "../../../contexts/shared/GlobalContext.tsx";
import {IMusicPlatformItem} from "../../../../models/music/IMusicPlatformItem.ts";
import {Container, Form, Button} from "react-bootstrap";

interface DeleteMusicPlatformProps {
    item: IMusicPlatformItem;
}

const DeleteMusicPlatformModal: FC<DeleteMusicPlatformProps> = ({item}) => {
    const globalContext = useContext(GlobalContext)!;
    const {sendRequest, OnHideModal, serverRoute} = globalContext!;
    const [musicPlatformId, setMusicPlatformId] = useState<string | null>(item.id.toString());

    useEffect(() => {
        setMusicPlatformId(item.id.toString());
    }, [item]);

    const handleDelete = async (e: React.FormEvent) => {
        try {
            e.preventDefault();

            const response = await sendRequest(
                `${serverRoute}/admin/music/${musicPlatformId}`,
                "DELETE",
            );

            if (response.status === 204) {
                alert("Музична платформа успішно видалена!");
                window.location.reload();
            } else {
                alert("Помилка під час видалення музичної платформи!");
            }
        } catch (error) {
            console.error("Помилка під час видалення музичної платформи:", error);
        }
        finally {
            OnHideModal();
        }
    };


    return (
        <Container className="form-modal">
            <h2 className="form-modal__title">Ви впевнені, що хочете видалити музичну платформу?</h2>
            <Form className="form-modal__form" onSubmit={handleDelete}>
                <Form.Group className="form-modal__group">
                    <Button className="form-modal__button form-modal__button--confirm" type="submit">
                        Так, видалити
                    </Button>
                    <Button className="form-modal__button form-modal__button--cancel" onClick={OnHideModal}>
                        Скасувати
                    </Button>
                </Form.Group>
            </Form>
        </Container>
    );
};

export {DeleteMusicPlatformModal};