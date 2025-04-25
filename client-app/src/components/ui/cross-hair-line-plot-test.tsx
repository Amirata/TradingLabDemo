import React, {useEffect, useRef, useMemo} from 'react';
import * as d3 from 'd3';
import {ChartBalance} from "../../libs/definitions.ts";
import {useAppStore} from "../../libs/stores/app-store.ts";
import apisWrapper from "../../libs/apis-wrapper.ts";
import {ChartLoader} from "./loading/page-loaders.tsx";

type LinePlotProps = {
    data: ChartBalance[];
    width?: number;
    height?: number;
    marginTop?: number;
    marginRight?: number;
    marginBottom?: number;
    marginLeft?: number;
};

const LineChart: React.FC<LinePlotProps> = ({
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
        const parseTime = d3.timeParse('%Y-%m-%dT%H:%M:%S');
        const result = data.map(d => {
            const parsedDate = parseTime(d.dateTime);
            if (!parsedDate) {
                console.error(`Failed to parse date: ${d.dateTime}`);
                return null;
            }
            return {date: parsedDate, balance: d.balance};
        }).filter(Boolean) as { date: Date; balance: number }[];
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

        // مقیاس‌بندی‌ها
        const xScale = d3.scaleTime()
            .domain(d3.extent(parsedData, d => d.date) as [Date, Date])
            .range([0, innerWidth]);

        const maxBalance = d3.max(parsedData, d => d.balance);
        const yScale = d3.scaleLinear()
            .domain([0, maxBalance ?? 0])
            .nice()
            .range([innerHeight, 0]);

        // خط ساز
        const line = d3.line<{ date: Date; balance: number }>()
            .x(d => xScale(d.date))
            .y(d => yScale(d.balance))
            .curve(d3.curveMonotoneX);

        // محورها
        g.append('g')
            .attr('transform', `translate(0,${innerHeight})`)
            .call(d3.axisBottom(xScale).ticks(Math.max(5, Math.floor(innerWidth / 100))).tickSizeOuter(0));

        g.append('g')
            .call(d3.axisLeft(yScale).ticks(5));

        // خط اصلی نمودار
        g.append('path')
            .datum(parsedData)
            .attr('fill', 'none')
            .attr('stroke', '#71717a')
            .attr('stroke-width', 1.3)
            .attr('d', line);

        // استایل‌دهی
        g.selectAll('.domain, line')
            .attr('stroke', '#71717a');
        g.selectAll('text')
            .attr('stroke', '#71717a')
            .style('font-family', 'JetBrainsMono-Regular');

        if (parsedData.length > 1000) {
            g.attr('shape-rendering', 'crispEdges');
        }

        // اضافه کردن Crosshair
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

        // گروه برای متن و پس‌زمینه محور X
        const xLabelGroup = focus.append('g')
            .attr('class', 'x-label-group');

        xLabelGroup.append('rect')
            .attr('class', 'x-label-bg')
            .style('fill', '#333') // رنگ پس‌زمینه، می‌توانید تغییر دهید
            .style('opacity', '0.8');

        xLabelGroup.append('text')
            .attr('class', 'x-label')
            .style('fill', '#fff') // رنگ متن سفید برای خوانایی
            .style('font-size', '12px')
            .attr('text-anchor', 'middle')
            .attr('dy', '1em');

        // گروه برای متن و پس‌زمینه محور Y
        const yLabelGroup = focus.append('g')
            .attr('class', 'y-label-group');

        yLabelGroup.append('rect')
            .attr('class', 'y-label-bg')
            .style('fill', '#333') // رنگ پس‌زمینه
            .style('opacity', '0.8');

        yLabelGroup.append('text')
            .attr('class', 'y-label')
            .style('fill', '#fff') // رنگ متن سفید
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
            const y = yScale(d.balance);

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

            // به‌روزرسانی متن و پس‌زمینه محور X
            const xLabel = focus.select('.x-label')
                .attr('x', x)
                .attr('y', innerHeight)
                .text(d3.timeFormat('%Y-%m-%d %H:%M')(d.date));

            const xLabelBBox = (xLabel.node() as SVGTextElement)?.getBBox();
            if (xLabelBBox) {
                focus.select('.x-label-bg')
                    .attr('x', xLabelBBox.x - 5)
                    .attr('y', xLabelBBox.y - 2)
                    .attr('width', xLabelBBox.width + 10)
                    .attr('height', xLabelBBox.height + 4);
            }

            // به‌روزرسانی متن و پس‌زمینه محور Y
            const yLabel = focus.select('.y-label')
                .attr('x', 0)
                .attr('y', y)
                .text(d.balance.toFixed(2));

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

const CrossHairLinePlotTest: React.FC = () => {
    const [loading, setLoading] = React.useState(true);
    const [data, setData] = React.useState<ChartBalance[]>([]);
    const {selectedPlan} = useAppStore();
    const planId = selectedPlan?.id;
    const {tradeDeleted} = useAppStore();

    useEffect(() => {
        const controller = new AbortController();
        if (!planId) {
            setLoading(false);
            return;
        }
        setLoading(true);
        apisWrapper.TradeAnalyseWrapper.getChartBalance(planId, {signal: controller.signal})
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
    }, [planId, tradeDeleted]);

    return (
        <div className="bg-zinc-900 m-5 aspect-[3/1] text-white/80 rounded-sm" dir="ltr">
            {loading && <ChartLoader/>}
            {!loading && data.length > 0 ? (
                <LineChart data={data}/>
            ) : (
                !loading &&
                <div className="flex h-full justify-center items-center font-iran-sans" dir="rtl">داده ای برای نمایش
                    وجود ندارد!</div>
            )}
        </div>
    );
};

export default CrossHairLinePlotTest;