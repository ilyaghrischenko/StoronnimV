import {FC} from "react";

interface IValidationErrorsProps {
    errors: Record<string, string[]>;
}

const ValidationErrors: FC<IValidationErrorsProps> = ({ errors }) => {
    return (
        <div className="d-flex justify-content-center align-items-center">
            <ul>
                {Object.entries(errors).map(([key, value]: [string, string[]]) => (
                    <li key={key}>
                        <strong>{key}:</strong>
                        <ul>
                            {(value as string[]).map((err, index) => (
                                <li key={index}>{err}</li>
                            ))}
                        </ul>
                    </li>
                ))}
            </ul>
        </div>
    );
};

export { ValidationErrors };