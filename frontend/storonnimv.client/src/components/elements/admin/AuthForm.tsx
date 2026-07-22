import {FC, FormEvent, useContext, useState} from "react";
import {Button, Form} from "react-bootstrap";
import {AdminContext} from "../../contexts/AdminContext.tsx";
import {ILogInRequest} from "../../../models/admin/ILogInRequest.ts";

const AuthForm: FC = () => {
    const adminContext = useContext(AdminContext);

    if (!adminContext) {
        throw new Error('AdminContext must be used within a AdminContextProvider');
    }

    const {logIn, loginError, isLoggingIn} = adminContext;

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
        <div className="auth-form">
            <Form
                onSubmit={handleSubmit}
                className='auth-form__form form-modal__form'
            >
                <Form.Group
                    controlId="admin-login"
                    className='auth-form__group form-modal__group'
                >
                    <Form.Label className='form-group__label'>Login:</Form.Label>
                    <Form.Control
                        className='form-modal__input'
                        type="text"
                        autoComplete="username"
                        onChange={(e) => setLogin(e.target.value)}
                        required
                    />
                </Form.Group>
                <Form.Group
                    className='auth-form__group form-modal__group'
                    controlId="admin-password"
                >
                    <Form.Label className='form-group__label'>Password:</Form.Label>
                    <Form.Control
                        className='form-modal__input'
                        type="password"
                        autoComplete="current-password"
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </Form.Group>
                <Button
                    className='form-modal__button form-modal__button--cancel'
                    variant="primary"
                    type="submit"
                    disabled={isLoggingIn}
                >
                    {isLoggingIn ? 'Вхід...' : 'Увійти'}
                </Button>
                {loginError && <p className="auth-form__error" role="alert" aria-live="assertive">{loginError}</p>}
            </Form>
        </div>
    );
};

export {AuthForm};
