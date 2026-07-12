import {FC} from "react";
import {Helmet} from "react-helmet-async";

const Developers: FC = () => {
    return (
        <>
            <Helmet>
                <title>Розробники - Стороннім В</title>
                <meta name="description" content="Дізнайтеся хто розробив сайт." />
            </Helmet>

            <p>hello</p>
        </>
    );
};

export { Developers };