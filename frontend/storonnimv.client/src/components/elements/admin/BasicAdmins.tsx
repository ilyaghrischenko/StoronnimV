import React, { useContext } from "react";
import { Button, Table } from "react-bootstrap";
import { GlobalContext } from "../../contexts/shared/GlobalContext";
import { AddAdminModal } from "./SuperAdminButtons/AddAdminModal.tsx";
import { EditAdminModal } from "./SuperAdminButtons/EditAdminModal.tsx";
import { DeleteAdminModal } from "./SuperAdminButtons/DeleteAdminModal.tsx";
import {IBasicAdmin} from "../../../models/admin/IBasicAdmin.ts";

interface BasicAdminsProps {
    admins: IBasicAdmin[];
    onAdding: (login: string, password: string) => Promise<boolean>;
    onDelete: (adminId: number) => Promise<boolean>;
    onLoginEdit: (adminId: number, newLogin: string) => Promise<boolean>;
    onPasswordEdit: (adminId: number, oldPassword: string, newPassword: string) => Promise<boolean>;
}

const BasicAdmins: React.FC<BasicAdminsProps> = ({ admins, onAdding, onDelete, onLoginEdit, onPasswordEdit }) => {
    const { OnShowModal, setValidationErrors } = useContext(GlobalContext)!;

    const handleAdd = () => {
        setValidationErrors({});
        OnShowModal(<AddAdminModal onAdding={onAdding} />);
    };

    const handleEdit = (admin: IBasicAdmin) => {
        setValidationErrors({});
        OnShowModal(<EditAdminModal admin={admin} onLoginEdit={onLoginEdit} onPasswordEdit={onPasswordEdit} />);
    };

    const handleDelete = (admin: IBasicAdmin) => {
        setValidationErrors({});
        OnShowModal(<DeleteAdminModal adminId={admin.id} onDelete={() => onDelete(admin.id)} />);
    };

    return (
        <div>
            <Button variant="primary" onClick={handleAdd}>
                Додати Адміна
            </Button>
            <h2 className="mt-3">Список Адмінів</h2>
            <Table striped bordered hover className="mt-3">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Логін</th>
                        <th>Дії</th>
                    </tr>
                </thead>
                <tbody>
                    {admins.map((admin) => (
                        <tr key={admin.id}>
                            <td>{admin.id}</td>
                            <td>{admin.login}</td>
                            <td>
                                <Button variant="warning" onClick={() => handleEdit(admin)}>
                                    Змінити
                                </Button>
                                <Button
                                    variant="danger"
                                    onClick={() => handleDelete(admin)}
                                >
                                    Видалити
                                </Button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </Table>
        </div>
    );
};

export { BasicAdmins };
