import {FC} from "react";
import {getValidationErrorId} from "./validationErrorUtils.ts";

interface IValidationErrorsProps {
    errors: Record<string, string[]>;
    idPrefix: string;
}

const ValidationErrors: FC<IValidationErrorsProps> = ({ errors, idPrefix }) => {
    return (
        <div className="validation-errors" role="alert" aria-live="assertive">
            <ul className="validation-errors__list">
                {Object.entries(errors).map(([key, value]: [string, string[]]) => (
                    <li
                        id={getValidationErrorId(idPrefix, key)}
                        className="validation-errors__field"
                        key={key}
                    >
                        <strong>{key}:</strong>
                        <ul className="validation-errors__messages">
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

export {ValidationErrors};
