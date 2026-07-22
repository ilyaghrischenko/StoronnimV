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
        <div className="admin-container">
            <Button className="admin-container__add" variant="primary" type="button" onClick={handleAdd}>
                Додати Адміна
            </Button>
            <h2 className="admin-container__heading">Список Адмінів</h2>
            <div className="admin-table-container">
                <Table striped bordered hover className="admin-table">
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
                                <td data-label="ID">{admin.id}</td>
                                <td data-label="Логін">{admin.login}</td>
                                <td data-label="Дії">
                                    <div className="admin-table__actions">
                                        <Button type="button" variant="warning" onClick={() => handleEdit(admin)}>
                                            Змінити
                                        </Button>
                                        <Button
                                            type="button"
                                            variant="danger"
                                            onClick={() => handleDelete(admin)}
                                        >
                                            Видалити
                                        </Button>
                                    </div>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </Table>
            </div>
        </div>
    );
};

export { BasicAdmins };
