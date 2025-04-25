import CalendarPart from "./calendar.tsx";
import {PlansCombobox} from "../../ui/my-comboboxes.tsx";
import Trades from "../trades/trades.tsx";
import CrossHairLinePlotTest from "../../ui/cross-hair-line-plot-test.tsx";
import {CalendarFooter} from "./calendar-footer.tsx";


function Main() {

    return (
        <div className="flex flex-col min-h-screen">
            <div className="flex min-h-screen gap-1">
                <div className=" flex flex-col w-full bg-default-100 rounded-lg pr-1">
                    <div className="flex justify-between items-center py-5 mb-2 px-5 bg-zinc-800 rounded-sm">
                        <div className="flex items-center justify-start">
                            <h1 className="text-sm text-white/80  pl-2 font-bold font-iran-sans">
                                پلن معاملاتی
                            </h1>
                            <PlansCombobox/>
                        </div>
                    </div>
                        <CrossHairLinePlotTest />
                        <CalendarPart />
                        <CalendarFooter />
                    <div className="bg-zinc-800 rounded-sm mt-1">
                        <Trades/>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Main;
