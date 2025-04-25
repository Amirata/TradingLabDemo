import React, {useEffect, useRef, useMemo} from 'react';
import {ChartNetProfit} from "../../../../libs/definitions.ts";
import apisWrapper from "../../../../libs/apis-wrapper.ts";
import {ChartLoader} from "../../../ui/loading/page-loaders.tsx";

import * as d3 from 'd3';
import {CircleHelp} from "lucide-react";
import {HelpModal} from "../../../ui/modals/Modal.tsx";
import {useParams} from "react-router-dom";


type BarChartProps = {
    data: ChartNetProfit[];
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
                                               marginRight = 20,
                                               marginBottom = 30,
                                               marginLeft = 80,
                                           }) => {
    const svgRef = useRef<SVGSVGElement>(null);

    const dimensions = useMemo(() => ({
        innerWidth: width - marginLeft - marginRight,
        innerHeight: height - marginTop - marginBottom,
    }), [width, height, marginLeft, marginRight, marginTop, marginBottom]);

    const parsedData = useMemo(() => {
        const parseDate = d3.timeParse('%Y-%m-%d');
        const result = data.map(d => {
            const parsedDate = parseDate(d.date);
            if (!parsedDate) {
                console.error(`Failed to parse date: ${d.date}`);
                return null;
            }
            return {date: parsedDate, netProfit: d.netProfit};
        }).filter(Boolean) as { date: Date; netProfit: number }[];
        return result;
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

        // محاسبه دامنه زمانی با فاصله اضافی
        const [minDate, maxDate] = d3.extent(parsedData, d => d.date) as [Date, Date];
        const timeDiff = maxDate.getTime() - minDate.getTime();
        const paddingTime = timeDiff * 0.1; // 10% فاصله در دو طرف
        const paddedMinDate = new Date(minDate.getTime() - paddingTime);
        const paddedMaxDate = new Date(maxDate.getTime() + paddingTime);

        // مقیاس‌بندی‌ها
        const xScale = d3.scaleTime()
            .domain([paddedMinDate, paddedMaxDate]) // دامنه با فاصله اضافی
            .range([0, innerWidth]);

        const yScale = d3.scaleLinear()
            .domain(d3.extent(parsedData, d => d.netProfit) as [number, number])
            .nice()
            .range([innerHeight, 0]);

        // تنظیم عرض میله‌ها
        const barWidth = Math.min(20, innerWidth / parsedData.length * 0.6);

        //#71717a 500 zinc
        //#3f3f46 700
        // میله‌ها
        g.selectAll('.bar')
            .data(parsedData)
            .join('rect')
            .attr('class', 'bar')
            .attr('x', d => xScale(d.date) - barWidth / 2)
            .attr('y', d => d.netProfit >= 0 ? yScale(d.netProfit) : yScale(0))
            .attr('width', barWidth)
            .attr('height', d => Math.abs(yScale(d.netProfit) - yScale(0)))
            .attr('fill', d => d.netProfit >= 0 ? '#71717a' : '#3f3f46');

        // محورها
        g.append('g')
            .attr('transform', `translate(0,${innerHeight})`)
            .call(d3.axisBottom(xScale).ticks(Math.max(5, Math.floor(innerWidth / 100))).tickSizeOuter(0))
            .call(g => g.select('.domain').attr('stroke', '#71717a'));

        g.append('g')
            .call(d3.axisLeft(yScale).ticks(5))
            .call(g => g.select('.domain').attr('stroke', '#71717a'));

        // خط صفر
        g.append('line')
            .attr('x1', 0)
            .attr('x2', innerWidth)
            .attr('y1', yScale(0))
            .attr('y2', yScale(0))
            .attr('stroke', '#71717a')
            .attr('stroke-width', 1);

        // استایل‌دهی
        g.selectAll('line')
            .attr('stroke', '#71717a');
        g.selectAll('text')
            .attr('stroke', '#71717a')
            .style('font-family', 'JetBrainsMono-Regular');

        // Crosshair
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

        const bisectDate = d3.bisector((d: { date: Date }) => d.date).left;

        function mousemove(event: any) {
            const [mouseX] = d3.pointer(event);
            const dateAtMouse = xScale.invert(mouseX);
            const index = bisectDate(parsedData, dateAtMouse);
            const d0 = parsedData[index - 1];
            const d1 = parsedData[index] || d0;
            const d = (dateAtMouse.getTime() - d0.date.getTime()) > (d1.date.getTime() - dateAtMouse.getTime()) ? d1 : d0;

            const x = xScale(d.date);
            const y = yScale(d.netProfit);

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
                .text(d3.timeFormat('%Y-%m-%d')(d.date));

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
                .text(d.netProfit.toFixed(2));

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

const GetChartNetProfit = () => {

    const [isModalOpen, setIsModalOpen] = React.useState<boolean>(false);
    const [loading, setLoading] = React.useState(true);
    const [data, setData] = React.useState<ChartNetProfit[]>([]);
    const {id} = useParams<{ id: string }>();

    useEffect(() => {
        const controller = new AbortController();
        if (!id) {
            setLoading(false);
            return;
        }
        setLoading(true);
        apisWrapper.TradeAnalyseWrapper.getChartNetProfit(id, {signal: controller.signal})
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

        return () => controller.abort();
    }, [id]);

    const message = useMemo(() =>
            <div className="text-sm text-white/80 text-justify font-iran-sans-fa-num max-w-[600px]">
                این نمودار برایند سود و زیان (NetProfit) در هر روز را نشان می دهد. به طور مثال اگر در یک روز 3 معامله انجام
                شده باشد، حاصل جمع سود و زیان آن معاملات در روز را نشان می دهد.
            </div>
        , [])

    return (
        <div className="flex flex-col bg-zinc-700 rounded-sm">
            <HelpModal
                setIsModalOpen={setIsModalOpen}
                isModalOpen={isModalOpen}
                message={message}

            />
            <div className="flex items-center pr-5 pt-5">
                <div className="flex items-center">
                    <h2 className="font-iran-sans text-white/80 ">سود و زیان روزانه</h2>
                    <CircleHelp className="mr-2 size-4 stroke-zinc-400 cursor-pointer"
                                onClick={() => setIsModalOpen(true)}/>
                </div>
            </div>

            <div className="bg-zinc-900 m-5 aspect-[3/1] text-white/80 rounded-sm" dir="ltr">
                {loading && <ChartLoader/>}
                {!loading && data.length > 0 ? (
                    <BarChart data={data}/>
                ) : (
                    !loading &&
                    <div className="flex h-full justify-center items-center font-iran-sans" dir="rtl">داده ای برای نمایش
                        وجود ندارد!</div>
                )}
            </div>
        </div>
    );
}

export default GetChartNetProfit;