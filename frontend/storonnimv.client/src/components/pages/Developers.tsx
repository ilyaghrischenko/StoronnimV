import {FC} from "react";
import {Helmet} from "react-helmet-async";

const Developers: FC = () => {
    return (
        <Helmet defer={false}>
            <title>Розробники - Стороннім В</title>
        </Helmet>
    );
};

export { Developers };
