import {FC, ReactNode} from "react";
import {Container} from "react-bootstrap";
import {AdminContextProvider} from "../contexts/AdminContext.tsx";

interface IAdminProps {
    children: ReactNode;
}

const Admin: FC<IAdminProps> = ({children}) => {
    return (
        <AdminContextProvider>
            <Container className='page'>
                <h1 className="visually-hidden-heading">Адміністрування</h1>
                {children}
            </Container>
        </AdminContextProvider>
    );
};

export {Admin};
