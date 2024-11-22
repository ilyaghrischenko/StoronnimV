import {Route, Routes} from "react-router-dom";

import {Schedule} from "./Schedule";
import {News} from "./News";
import {Music} from "./Music";
import {Group} from "./Group";
import {Contacts} from "./Contacts";
import {Home} from "./Home";

const Page = () => {
    return (
        <Routes>
            <Route path="/" element={<Home/>}/>
            <Route path="/schedule" element={<Schedule/>}/>
            <Route path="/news" element={<News/>}/>
            <Route path="/music" element={<Music/>}/>
            <Route path="/group" element={<Group/>}/>
            <Route path="/contacts" element={<Contacts/>}/>
        </Routes>
    );
};

export {Page};