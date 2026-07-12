import {FC, FormEvent, useContext, useState} from "react";
import {Button, Form} from "react-bootstrap";
import {AdminContext} from "../../contexts/AdminContext.tsx";
import {ILogInRequest} from "../../../models/admin/ILogInRequest.ts";

const AuthForm: FC = () => {
    const adminContext = useContext(AdminContext);

    if (!adminContext) {
        throw new Error('AdminContext must be used within a AdminContextProvider');
    }

    const {logIn} = adminContext;

    const [login, setLogin] = useState<string>('');
    const [password, setPassword] = useState<string>('');

    const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        const logInRequest: ILogInRequest = {
            login: login,
            password: password
        }

        await logIn(logInRequest);
    };

    return (
        <div>
            <Form
                onSubmit={handleSubmit}
                className='form-modal__form'
            >
                <Form.Group
                    controlId="formAdminLogin"
                    className='form-modal__group'
                >
                    <Form.Label className='form-group__label'>Login:</Form.Label>
                    <Form.Control
                        className='form-modal__input'
                        type="text"
                        onChange={(e) => setLogin(e.target.value)}
                        required
                    />
                </Form.Group>
                <Form.Group
                    className='form-modal__group'
                    controlId="formAdminPassword"
                >
                    <Form.Label className='form-group__label'>Password:</Form.Label>
                    <Form.Control
                        className='form-modal__input'
                        type="password"
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </Form.Group>
                <Button
                    className='form-modal__button form-modal__button--cancel'
                    variant="primary"
                    type="submit"
                >
                    Увійти
                </Button>
            </Form>
        </div>
    );
};

export {AuthForm};