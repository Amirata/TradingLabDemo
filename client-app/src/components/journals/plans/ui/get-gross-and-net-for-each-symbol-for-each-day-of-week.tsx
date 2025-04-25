import React, {useEffect, useRef, useMemo, useState, useCallback} from 'react';
import {
    DayOfWeekEnum,
    GrossAndNetForEachSymbolForEachDayOfWeek,
} from "../../../../libs/definitions.ts";

import apisWrapper from "../../../../libs/apis-wrapper.ts";
import {ChartLoader} from "../../../ui/loading/page-loaders.tsx";

import * as d3 from 'd3';
import {CircleHelp} from "lucide-react";
import {HelpModal} from "../../../ui/modals/Modal.tsx";
import {useParams} from "react-router-dom";
import {getDateRange, getEnumName} from "../../../../libs/utilities.ts";
import {SymbolsDropdown} from "../../../ui/my-drop-downs.tsx";
import {useAppStore} from "../../../../libs/stores/app-store.ts";
import debounce from "lodash/debounce";

type props = {
    startDate: string;
    endDate: string;
    sendToServer: (start: string, end: string) => void;
    onRangeChange: (start: string, end: string) => void;
}

const RangeSlider: React.FC<props> = ({sendToServer, startDate, endDate, onRangeChange}) => {
    const startDateObj = new Date(startDate);
    const endDateObj = new Date(endDate);
    const totalDays = Math.floor((endDateObj.getTime() - startDateObj.getTime()) / (1000 * 60 * 60 * 24));

    const [range, setRange] = useState<[number, number]>([0, totalDays]);
    const sliderRef = useRef<HTMLDivElement>(null);

    const getDateFromValue = useCallback((value: number): string => {
        const newDate = new Date(startDateObj);
        newDate.setDate(startDateObj.getDate() + value);
        return newDate.toISOString().split("T")[0];
    }, [startDateObj]);

    const currentStartDate = useMemo(() => getDateFromValue(range[0]), [range, getDateFromValue]);
    const currentEndDate = useMemo(() => getDateFromValue(range[1]), [range, getDateFromValue]);
    const minThumbPosition = (range[0] / totalDays) * 100;
    const maxThumbPosition = (range[1] / totalDays) * 100;

    // Debounce کردن sendToServer برای جلوگیری از درخواست‌های مکرر
    const debouncedSendToServer = useMemo(() =>
            debounce((start: string, end: string) => {
                sendToServer(start, end);
                onRangeChange(start, end);
            }, 500),
        [sendToServer, onRangeChange]
    );

    useEffect(() => {
        debouncedSendToServer(currentStartDate, currentEndDate);
        return () => debouncedSendToServer.cancel(); // تمیز کردن debounce
    }, [currentStartDate, currentEndDate, debouncedSendToServer]);

    const handleMouseDown = useCallback(
        (thumb: "min" | "max") => () => {
            if (!sliderRef.current) return;
            const rect = sliderRef.current.getBoundingClientRect();

            const moveHandler = (moveEvent: MouseEvent) => {
                const newValue = Math.round(((moveEvent.clientX - rect.left) / rect.width) * totalDays);
                const clampedValue = Math.max(0, Math.min(totalDays, newValue));
                setRange(([min, max]) =>
                    thumb === "min" && clampedValue < max ? [clampedValue, max] : clampedValue > min ? [min, clampedValue] : [min, max]
                );
            };

            const upHandler = () => {
                document.removeEventListener("mousemove", moveHandler);
                document.removeEventListener("mouseup", upHandler);
            };

            document.addEventListener("mousemove", moveHandler);
            document.addEventListener("mouseup", upHandler);
        },
        [totalDays]
    );

    return (
        <div className="flex flex-col items-center justify-center">
            <div className="w-full p-6">
                <div ref={sliderRef} className="relative w-full h-2 bg-zinc-600 rounded-full">
                    <div className="absolute h-2 bg-zinc-800 rounded-full"
                         style={{left: `${minThumbPosition}%`, width: `${maxThumbPosition - minThumbPosition}%`}}/>
                    <div
                        className="absolute w-5 h-5 bg-zinc-400 rounded-full cursor-pointer transform -translate-x-1/2 -translate-y-1.5"
                        style={{left: `${minThumbPosition}%`}} onMouseDown={handleMouseDown("min")}/>
                    <div
                        className="absolute w-5 h-5 bg-zinc-400 rounded-full cursor-pointer transform -translate-x-1/2 -translate-y-1.5"
                        style={{left: `${maxThumbPosition}%`}} onMouseDown={handleMouseDown("max")}/>
                </div>
                <div
                    className="flex items-center justify-between mt-5 text-xs text-white/80 font-jbm-regular select-none"
                    dir="ltr">
                    <p>{currentStartDate}</p>
                    <p>{currentEndDate}</p>
                </div>
            </div>
        </div>
    );
};


type BarChartData = {
    label: string;
    values: { name: string; value: number; color: string }[];
};

type BarChartProps = {
    data: BarChartData[];
    width?: number;
    height?: number;
    marginTop?: number;
    marginRight?: number;
    marginBottom?: number;
    marginLeft?: number;
};

const BarChart: React.FC<BarChartProps> = ({
                                               data,
                                               width = 1200,
                                               height = 400,
                                               marginTop = 20,
                                               marginRight = 150,
                                               marginBottom = 30,
                                               marginLeft = 80,
                                           }) => {
    const svgRef = useRef<SVGSVGElement>(null);

    const dimensions = useMemo(() => ({
        innerWidth: width - marginLeft - marginRight,
        innerHeight: height - marginTop - marginBottom,
    }), [width, height, marginLeft, marginRight, marginTop, marginBottom]);

    const parsedData = useMemo(() => {
        return data.map(d => ({label: d.label, values: d.values}));
    }, [data]);

    useEffect(() => {
        if (!svgRef.current || !parsedData.length) return;

        const {innerWidth, innerHeight} = dimensions;

        const svg = d3.select(svgRef.current);
        svg.selectAll('*').remove();
        svg.attr('viewBox', `0 0 ${width} ${height}`)
            .attr('width', width)
            .attr('height', height)
            .attr('style', 'max-width: 100%; height: auto; font: 10px JetBrainsMono-Regular;')
            .style("overflow", "visible");

        const g = svg.append('g')
            .attr('transform', `translate(${marginLeft},${marginTop})`);


        const allValues = parsedData.flatMap(d => d.values.map(v => v.value));
        const yDomain = d3.extent(allValues) as [number, number];

// اطمینان از حضور صفر در دامنه
        if (yDomain[0] > 0) yDomain[0] = 0;
        if (yDomain[1] < 0) yDomain[1] = 0;

        const yScale = d3.scaleLinear()
            .domain(yDomain)
            .nice()
            .range([innerHeight, 0]);

        const xScale = d3.scaleBand()
            .domain(parsedData.map(d => d.label))
            .range([0, innerWidth])
            .padding(0.2);

        const barWidth = xScale.bandwidth() / parsedData[0].values.length;

        const barGroups = g.selectAll('.bar-group')
            .data(parsedData)
            .join('g')
            .attr('class', 'bar-group')
            .attr('transform', d => `translate(${xScale(d.label)},0)`);

        barGroups.selectAll('.bar')
            .data(d => d.values)
            .join('rect')
            .attr('class', 'bar')
            .attr('x', (_, i) => i * barWidth)
            .attr('y', d => d.value >= 0 ? yScale(d.value) : yScale(0))
            .attr('width', barWidth * 0.9)
            .attr('height', d => Math.abs(yScale(d.value) - yScale(0)))
            .attr('fill', d => d.color);

        g.append('g')
            .attr('transform', `translate(0,${innerHeight})`)
            .call(d3.axisBottom(xScale).tickSizeOuter(0))
            .call(g => g.select('.domain').attr('stroke', '#71717a'));

        g.append('g')
            .call(d3.axisLeft(yScale).ticks(5))
            .call(g => g.select('.domain').attr('stroke', '#71717a'));

        g.append('g')
            .append('line')
            .attr('x1', 0)
            .attr('x2', innerWidth)
            .attr('y1', yScale(0))
            .attr('y2', yScale(0))
            .attr('stroke', '#71717a')
            .attr('stroke-width', 1);

        g.selectAll('line')
            .attr('stroke', '#71717a');
        g.selectAll('text')
            .attr('stroke', '#71717a')
            .style('font-family', 'JetBrainsMono-Regular');

        const legend = svg.append('g')
            .attr('transform', `translate(${width - marginRight + 10}, ${marginTop})`);

        const uniqueBars = parsedData[0].values;
        legend.selectAll('.legend-item')
            .data(uniqueBars)
            .join('g')
            .attr('class', 'legend-item')
            .attr('transform', (_, i) => `translate(0, ${i * 20})`)
            .each(function (d) {
                const group = d3.select(this);
                group.append('rect')
                    .attr('x', 0)
                    .attr('y', 0)
                    .attr('width', 15)
                    .attr('height', 15)
                    .attr('fill', d.color);

                group.append('text')
                    .attr('x', 20)
                    .attr('y', 12)
                    .text(d.name)
                    .style('fill', '#71717a')
                    .style('font-family', 'JetBrainsMono-Regular')
                    .style('font-size', '12px');
            });

        const focus = g.append('g')
            .attr('class', 'focus')
            .style('display', 'none');

        focus.append('line')
            .attr('class', 'x-line')
            .style('stroke', '#71717a')
            .style('stroke-width', '1px')
            .style('stroke-dasharray', '3,3');

        focus.append('line')
            .attr('class', 'y-line')
            .style('stroke', '#71717a')
            .style('stroke-width', '1px')
            .style('stroke-dasharray', '3,3');

        const xLabelGroup = focus.append('g').attr('class', 'x-label-group');
        xLabelGroup.append('rect')
            .attr('class', 'x-label-bg')
            .style('fill', '#333')
            .style('opacity', '0.8');
        xLabelGroup.append('text')
            .attr('class', 'x-label')
            .style('fill', '#fff')
            .style('font-size', '12px')
            .attr('text-anchor', 'middle')
            .attr('dy', '1em');

        const yLabelGroup = focus.append('g').attr('class', 'y-label-group');
        yLabelGroup.append('rect')
            .attr('class', 'y-label-bg')
            .style('fill', '#333')
            .style('opacity', '0.8');
        yLabelGroup.append('text')
            .attr('class', 'y-label')
            .style('fill', '#fff')
            .style('font-size', '12px')
            .attr('text-anchor', 'end')
            .attr('dx', '-0.5em');

        g.append('rect')
            .attr('class', 'overlay')
            .attr('width', innerWidth)
            .attr('height', innerHeight)
            .style('fill', 'none')
            .style('pointer-events', 'all')
            .on('mouseover', () => focus.style('display', null))
            .on('mouseout', () => focus.style('display', 'none'))
            .on('mousemove', mousemove);

        function mousemove(event: any) {
            const [mouseX] = d3.pointer(event);

            // بررسی اینکه ماوس در محدوده چارت است یا خیر
            if (mouseX < 0 || mouseX > innerWidth) {
                focus.style('display', 'none');
                return;
            }

            const labelIndex = Math.floor(mouseX / xScale.step());
            if (labelIndex < 0 || labelIndex >= parsedData.length) {
                focus.style('display', 'none');
                return;
            }

            const labelAtMouse = xScale.domain()[labelIndex];
            const groupData = parsedData.find(item => item.label === labelAtMouse) || parsedData[0];
            const groupX = xScale(labelAtMouse) || 0;
            const relativeX = mouseX - groupX;
            const barIndex = Math.min(
                Math.max(Math.floor(relativeX / barWidth), 0),
                groupData.values.length - 1
            );
            const d = groupData.values[barIndex];

            const x = groupX + (barIndex * barWidth) + barWidth / 2;
            const y = yScale(d.value);

            focus.style('display', null); // نمایش Crosshair
            focus.select('.x-line')
                .attr('x1', x)
                .attr('y1', 0)
                .attr('x2', x)
                .attr('y2', innerHeight);

            focus.select('.y-line')
                .attr('x1', 0)
                .attr('y1', y)
                .attr('x2', innerWidth)
                .attr('y2', y);

            const xLabel = focus.select('.x-label')
                .attr('x', x)
                .attr('y', innerHeight)
                .text(`${groupData.label} - ${d.name}`);

            const xLabelBBox = (xLabel.node() as SVGTextElement)?.getBBox();
            if (xLabelBBox) {
                focus.select('.x-label-bg')
                    .attr('x', xLabelBBox.x - 5)
                    .attr('y', xLabelBBox.y - 2)
                    .attr('width', xLabelBBox.width + 10)
                    .attr('height', xLabelBBox.height + 4);
            }

            const yLabel = focus.select('.y-label')
                .attr('x', 0)
                .attr('y', y)
                .text(d.value.toFixed(2));

            const yLabelBBox = (yLabel.node() as SVGTextElement)?.getBBox();
            if (yLabelBBox) {
                focus.select('.y-label-bg')
                    .attr('x', yLabelBBox.x - 5)
                    .attr('y', yLabelBBox.y - 2)
                    .attr('width', yLabelBBox.width + 10)
                    .attr('height', yLabelBBox.height + 4);
            }
        }
    }, [parsedData, dimensions]);

    return <svg ref={svgRef}/>;
};

type barChartProp = {
    data: GrossAndNetForEachSymbolForEachDayOfWeek[];
}
const BarChartCreate: React.FC<barChartProp> = ({data}) => {
    //#71717a 500 zinc
    //#3f3f46 700


    const chartData = data.map<BarChartData>((item) => {
        return {
            label: getEnumName(DayOfWeekEnum, Number(item.dayOfWeek)) as string,
            values: [
                {name: 'Net Profit', value: item.netProfit, color: "#71717a"},
                {name: 'Gross Profit', value: item.grossProfit, color: "#3f3f46"},
            ]
        }
    })

    return (
        <div className="bg-zinc-900 m-5 aspect-[3/1] text-white/80 rounded-sm" dir="ltr">
            <BarChart data={chartData}/>
        </div>
    );
};

const GetGrossAndNetForEachSymbolForEachDayOfWeek = () => {
    const [isModalOpen, setIsModalOpen] = useState<boolean>(false);
    const [loading, setLoading] = useState(true);
    const [data, setData] = useState<GrossAndNetForEachSymbolForEachDayOfWeek[]>([]);
    const [years, setYears] = useState<{ startDate: string, endDate: string } | null>(null);
    const [currentRange, setCurrentRange] = useState<{ start: string, end: string } | null>(null);
    const {id} = useParams<{ id: string }>();
    const {selectedSymbol} = useAppStore();

    const sendToServer = useCallback((start: string, end: string) => {

        setLoading(true);
        apisWrapper.TradeAnalyseWrapper.getGrossAndNetForEachSymbolForEachDayOfWeek(id as string, start, end, selectedSymbol)
            .then((r) => {
                if (r) {
                    setData(r);
                    setLoading(false);
                }
            })
            .catch((err) => {
                if (err.name !== 'AbortError') {
                    // console.error('API Error:', err);
                }
                setLoading(false);
            });
    }, [id, selectedSymbol]);

    const handleRangeChange = useCallback((start: string, end: string) => {
        setCurrentRange({start, end});
    }, []);

    // دریافت سال‌ها فقط یک بار در لود اولیه
    useEffect(() => {
        const controller = new AbortController();
        if (!id) {
            setLoading(false);
            return;
        }
        setLoading(true);

        apisWrapper.TradeAnalyseWrapper.getGetTradeYears(id).then((yearsData) => {
            if (yearsData.length === 0) {
                setLoading(false);
                return;
            }
            const {startDate, endDate} = getDateRange(yearsData[yearsData.length - 1], yearsData[0]);
            setYears({startDate, endDate});
            setCurrentRange({start: startDate, end: endDate}); // تنظیم اولیه currentRange
        }).catch((err) => {
            if (err.name !== 'AbortError') {
                // console.error('API Error:', err);
            }
            setLoading(false);
        });

        return () => controller.abort();
    }, [id]);

    // دریافت داده‌ها هنگام تغییر selectedSymbol یا currentRange
    useEffect(() => {
        if (!years || !currentRange) return; // منتظر می‌مانیم تا years و currentRange آماده شوند
        sendToServer(currentRange.start, currentRange.end);
    }, [selectedSymbol, currentRange, years, sendToServer]);

    const message = useMemo(() =>
            <div className="text-sm text-white/80 text-justify font-iran-sans-fa-num max-w-[600px]">
                این نمودار سود و زیان کلی را در بازه تاریخی به ازای هر نماد در روزهای هفته نشان می دهد.
            </div>
        , []);

    return (
        <div className="flex flex-col bg-zinc-700 rounded-sm">
            <HelpModal
                setIsModalOpen={setIsModalOpen}
                isModalOpen={isModalOpen}
                message={message}
            />
            <div className="flex flex-col md:flex-row md:items-center pr-5 pt-5 ">
                <div className="flex items-center md:basis-1/2 lg:basis-3/4">
                    <h2 className="font-iran-sans text-white/80 ">سود و زیان کلی به ازای هر نماد در روزهای هفته</h2>
                    <CircleHelp className="mr-2 size-4 stroke-zinc-400 cursor-pointer"
                                onClick={() => setIsModalOpen(true)}/>
                </div>
                <div className="md:basis-1/2 lg:basis-1/4 pl-5 pt-3 md:pt-0">
                    <SymbolsDropdown/>
                </div>
            </div>

            <div className="bg-zinc-900 m-5 aspect-[3/1] text-white/80 rounded-sm" dir="ltr">
                {loading && <ChartLoader/>}
                {!loading && data.length > 0 ? (
                    <BarChartCreate data={data}/>
                ) : (
                    !loading &&
                    <div className="flex h-full justify-center items-center font-iran-sans" dir="rtl">داده ای برای نمایش
                        وجود ندارد!</div>
                )}
            </div>
            {years &&
                <RangeSlider
                    sendToServer={sendToServer}
                    startDate={years.startDate}
                    endDate={years.endDate}
                    onRangeChange={handleRangeChange}
                />}
        </div>
    );
}

export default GetGrossAndNetForEachSymbolForEachDayOfWeek;