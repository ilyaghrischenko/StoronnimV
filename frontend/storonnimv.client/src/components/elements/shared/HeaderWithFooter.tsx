import {Header} from "./Header.tsx";
import {Footer} from "./Footer.tsx";

const HeaderWithFooter = () => {
    return (
        <div className='header-with-footer-container'>
            <Header/>
            <Footer/>
        </div>
    );
};

export { HeaderWithFooter };