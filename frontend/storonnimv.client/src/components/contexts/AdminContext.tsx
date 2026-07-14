import {createContext, FC, ReactNode, useContext, useState} from "react";
import {GlobalContext} from "./shared/GlobalContext.tsx";
import {ILogInRequest} from "../../models/admin/ILogInRequest.ts";
import {useNavigate} from "react-router-dom";
import {IBasicAdmin} from "../../models/admin/IBasicAdmin.ts";

interface AdminContextType {
    logIn: (logInRequest: ILogInRequest) => Promise<void>;
    loginError: string;
    isLoggingIn: boolean;
    deleteAdmin: (adminId: number) => Promise<void>;
    basicAdmins: IBasicAdmin[];
    fetchBasicAdmins: () => Promise<void>;
    addAdmin: (login: string, password: string) => Promise<void>;
    editAdminLogin: (adminId: number, newLogin: string) => Promise<void>;
    editAdminPassword: (adminId: number, oldPassword: string, newPassword: string) => Promise<void>;
}

const AdminContext = createContext<AdminContextType | undefined>(undefined);

interface AdminContextProviderProps {
    children: ReactNode;
}

const AdminContextProvider: FC<AdminContextProviderProps> = ({children}) => {
    const globalContext = useContext(GlobalContext)!;

    const {sendRequest, setIsAdmin, setValidationErrors, serverRoute} = globalContext;
    const navigate = useNavigate();
    const [loginError, setLoginError] = useState<string>('');
    const [isLoggingIn, setIsLoggingIn] = useState<boolean>(false);

    const logIn = async (logInRequest: ILogInRequest) => {
        setLoginError('');
        setIsLoggingIn(true);

        try {
            const response = await sendRequest(
                `${serverRoute}/account/login`,
                'POST',
                JSON.stringify({login: logInRequest.login, password: logInRequest.password}),
                {'Content-Type': 'application/json'}
            );

            if (response.status === 200) {
                setIsAdmin(true);

                const adminRole: string = response.data;
                sessionStorage.setItem('role', adminRole);
                navigate('/', {replace: true});
                return;
            }

            if (response.status === 400) {
                setLoginError('Перевірте логін і пароль.');
            } else if (response.status === 401) {
                setLoginError('Неправильний логін або пароль.');
            } else {
                setLoginError('Не вдалося увійти. Спробуйте ще раз.');
            }
        } catch (error) {
            console.error(`Error while logging in: ${error}`);
            setLoginError('Сервер недоступний. Спробуйте ще раз.');
        } finally {
            setIsLoggingIn(false);
        }
    };

    const [basicAdmins, setBasicAdmins] = useState([{} as IBasicAdmin]);

    const fetchBasicAdmins = async () => {
        try {
            const response = await sendRequest(`${serverRoute}/super-admin/basic-admins`);

            if (response.status === 200) {
                const data: IBasicAdmin[] = response.data;
                setBasicAdmins(data);
            }
        } catch (error) {
            console.error(`Error while fetching basic admins: ${error}`);
        }
    };

    const addAdmin = async (login: string, password: string) => {
        alert('admin adding!');
        try {
            const response = await sendRequest(
                `${serverRoute}/super-admin/basic-admins`,
                "POST",
                JSON.stringify({login, password}),
                {"Content-Type": "application/json"}
            );

            if (response.status === 200) {
                const addedAdmin: IBasicAdmin = response.data;
                setBasicAdmins((prevAdmins) => [...prevAdmins, addedAdmin]);

            } else if (response.status === 400) {
                setValidationErrors(response.data.errors);
            }
        } catch (error) {
            console.error(`Error while adding basic admin: ${error}`);
        }
    };

    const deleteAdmin = async (adminId: number) => {
        try {
            const response = await sendRequest(
                `${serverRoute}/super-admin/basic-admins/${adminId}`,
                'DELETE'
            );

            if (response.status === 204) {
                basicAdmins.filter(admin => admin.id != adminId);
            }
        } catch (error) {
            console.error(`Error while deleting admin: ${error}`);
        }
    };

    const editAdminLogin = async (adminId: number, newLogin: string) => {
        try {
            const response = await sendRequest(
                `${serverRoute}/super-admin/basic-admins/${adminId}/login`,
                'PATCH',
                JSON.stringify({newLogin}),
                {'Content-Type': 'application/json'}
            );

            if (response.status === 200) {
                const updatedAdmin: IBasicAdmin = response.data;

                setBasicAdmins((prevAdmins) =>
                    prevAdmins.map((admin) =>
                        admin.id === adminId ? {...admin, login: updatedAdmin.login} : admin
                    )
                );

                alert('ДАНІ АДМІНА ЗМІНЕНІ');
            } else if (response.status === 400) {
                setValidationErrors(response.data.errors);
            }
        } catch (error) {
            console.error(`Error while editing admin login: ${error}`);
        }
    };

    const editAdminPassword = async (adminId: number, oldPassword: string, newPassword: string) => {
        if (oldPassword !== newPassword) {
            return;
        }

        try {
            const response = await sendRequest(
                `${serverRoute}/super-admin/basic-admins/${adminId}/password`,
                'PATCH',
                JSON.stringify({oldPassword, newPassword}),
                {'Content-Type': 'application/json'}
            );

            if (response.status === 400) {
                setValidationErrors(response.data.errors);
            }
        } catch (error) {
            console.error(`Error while editing admin password: ${error}`);
        }
    };

    const value: AdminContextType = {
        logIn,
        loginError,
        isLoggingIn,
        deleteAdmin,
        basicAdmins,
        fetchBasicAdmins,
        addAdmin,
        editAdminLogin,
        editAdminPassword,
    };

    return (
        <AdminContext.Provider value={value}>
            {children}
        </AdminContext.Provider>
    );
};

export {AdminContextProvider, AdminContext};
