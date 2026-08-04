import {FC, useContext, useEffect} from "react";
import { BasicAdmins } from "./BasicAdmins.tsx";
import {AdminContext} from "../../contexts/AdminContext.tsx";

const AdminContainer: FC = () => {
    const adminContext = useContext(AdminContext);

    if (!adminContext) {
        throw new Error("AdminContext must be used within a AdminContextProvider");
    }

    const {basicAdmins, fetchBasicAdmins, addAdmin, deleteAdmin, editAdminLogin, editAdminPassword} = adminContext;

    useEffect(() => {
        void fetchBasicAdmins();
    }, [fetchBasicAdmins]);

    return (
        <BasicAdmins
            admins={basicAdmins}
            onAdding={addAdmin}
            onDelete={deleteAdmin}
            onLoginEdit={editAdminLogin}
            onPasswordEdit={editAdminPassword} />
    );
};

export { AdminContainer };
