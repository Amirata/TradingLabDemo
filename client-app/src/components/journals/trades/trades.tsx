import { useEffect, useState, useCallback } from "react";
import { Link } from "react-router-dom";

import Table, { Config } from "../../ui/tables/Table";
import {SquareBtn, TableAddBtn} from "../../ui/my-buttons.tsx";
import apisWrapper from "../../../libs/apis-wrapper.ts";
import { useAppStore } from "../../../libs/stores/app-store.ts";

import {
    Sort,
    PaginationDto,
    PaginatedResult,
    Trade,
    QueryIdDto, SelectedPlan, Symbols, PositionType,
} from "../../../libs/definitions.ts";
import {formatDate, formatDateTime, formatNumber, getEnumName} from "../../../libs/utilities.ts";
import {DeleteIcon, EditIcon, EyeIcon} from "../../icons.tsx";
import {DeleteModal} from "../../ui/modals/Modal.tsx";
import {SquarePlus} from "lucide-react";

const useTradesData = (selectedPlan: SelectedPlan | null) => {
    const [page, setPage] = useState<PaginationDto>({ pageSize: 5, pageNumber: 1 });
    const [search, setSearch] = useState<string>("");
    const [data, setData] = useState<PaginatedResult<Trade> | undefined>();
    const [isLoading, setIsLoading] = useState(false);
    const [sorts, setSorts] = useState<Sort[]>([]);
    const {tradeDeleted} = useAppStore();

    useEffect(() => {
        if (!selectedPlan?.id) return;

        setIsLoading(true);
        const queryDto: QueryIdDto<string> = { ...page, search, sorts, id: selectedPlan.id };

        apisWrapper.TradeWrapper.list(queryDto)
            .then(setData)
            .catch(console.error)
            .finally(() => setIsLoading(false));
    }, [page, search, sorts, selectedPlan, tradeDeleted]);

    return { data, isLoading, setPage, setSearch, setSorts, search, sorts, page };
};

const useTradeDelete = (setPage: (setter: (prev: PaginationDto) => PaginationDto) => void,
                         currentPage: PaginationDto,
                         dataLength: number,
                         //setUpdateTableData: (setter: (prev: boolean) => boolean) => void // اضافه کردن setUpdateTableData
)=> {
    const [deleteTrade, setDeleteTrade] = useState<Trade | undefined>(undefined);
    const [deleteLoading, setDeleteLoading] = useState<boolean>(false);
    const [isModalOpen, setIsModalOpen] = useState<boolean>(false);

    const {setTradeDeleted} = useAppStore();

    const handleDeleteAlert = <T extends Trade>(obj: T) => {
        setDeleteTrade(obj);
        setIsModalOpen(true);
    };

    const handleDelete = useCallback(() => {
        if (!deleteTrade) return;
        setDeleteLoading(true);
        apisWrapper.TradeWrapper.delete(deleteTrade.id)
            .then(() => {
                if (dataLength === 1 && currentPage.pageNumber > 1) {
                    setPage((prev) => ({
                        ...prev,
                        pageNumber: prev.pageNumber - 1,
                    }));
                }
                //setUpdateTableData((prev) => !prev); // استفاده از setUpdateTableData که از Plans اومده
                setTradeDeleted();
            })
            .catch((error) => {
                console.error("Error deleting Plan:", error);
            })
            .finally(() => {
                setDeleteLoading(false);
                setIsModalOpen(false);
            });
    }, [deleteTrade, setPage, currentPage, dataLength]);

    return { deleteLoading, handleDeleteAlert, handleDelete, isModalOpen, setIsModalOpen, deleteTrade };
}

const Trades = () => {
    const { selectedPlan } = useAppStore();

    const { data, isLoading, setPage, setSearch, setSorts, search, sorts, page } = useTradesData(selectedPlan);

    const {
        deleteLoading,
        handleDeleteAlert,
        handleDelete,
        isModalOpen,
        setIsModalOpen,
        deleteTrade,
    } = useTradeDelete(setPage, page, data?.data.length || 0); // پاس دادن setUpdateTableData


    const handlePageClick = useCallback((pageNumber: number) => {
        setPage((prev) => ({ ...prev, pageNumber }));
    }, [setPage]);

    const handleSearch = useCallback((search: string) => {
        setSearch(search);
        setPage((prev) => ({ ...prev, pageNumber: 1 }));
    }, [setSearch, setPage]);

    const handleHeaderSortClick = useCallback((sortName: string) => {
        setSorts((prevSorts) => {
            const existingSort = prevSorts.find((sort) => sort.sortBy === sortName);
            const otherSorts = prevSorts.filter((sort) => sort.sortBy !== sortName)
                .map((sort, index) => ({ ...sort, order: index + 1 }));

            if (!existingSort) {
                return [...otherSorts, { sortBy: sortName, sortOrder: "asc", order: otherSorts.length + 1 }];
            }
            return existingSort.sortOrder === "asc"
                ? [...otherSorts, { sortBy: sortName, sortOrder: "desc", order: otherSorts.length + 1 }]
                : otherSorts;
        });
    }, [setSorts]);

    const tableOperation = (id: string | number, paths: string[], onDeleteFunc: <T, >(obj: T) => void) => (
        <div className="flex items-center justify-end">
            {paths.map((path, index) => {
                if (path === "delete") {
                    return (
                        <SquareBtn key={index} message={<DeleteIcon/>} onClick={onDeleteFunc}/>
                    )
                } else {
                    return (

                        <Link key={index} to={`${path}/${id}`}>
                            <SquareBtn message={[<EyeIcon/>, <EditIcon/>][index]}/>
                        </Link>
                    )
                }
            })}
        </div>
    );

    const config: Array<Config<Trade>> = [
        {
            key: 1,
            width: "w-[8%]",
            label: "نماد",
            render: (c) => <span dir="ltr" className="font-jbm-regular">{getEnumName(Symbols, Number(c.symbol))}</span>
        },
        {
            key: 2,
            width: "w-[8%]",
            label: "جهت",
            render: (c) => <span dir="ltr"
                                 className="font-jbm-regular">{getEnumName(PositionType, Number(c.positionType))}</span>
        },
        {
            key: 3,
            width: "w-[10%]",
            label: "تاریخ ورود",
            render: (c) => <span dir="ltr" className="font-jbm-regular">{formatDate(c.entryDateTime)}</span>,
            sortName: "EntryDateTime",
            direction: "text-left" },
        { key: 4, width:"w-[10%]", label: "تاریخ خروج", render: (c) => <span dir="ltr" className="font-jbm-regular">{formatDate(c.closeDateTime)}</span>, sortName: "CloseDateTime", direction: "text-left" },
        { key: 5, width:"w-[15%]", label: "سود خالص", render: (c) => <span dir="ltr" className="font-jbm-regular">{formatNumber(c.netProfit)}</span>, sortName: "NetProfit", direction: "text-left" },
        { key: 6, width:"w-[15%]", label: "پیپ", render: (c) => <span dir="ltr" className="font-jbm-regular">{formatNumber(c.pips)}</span>, sortName: "Pips", direction: "text-left" },
        { key: 7, width:"w-[20%]", label: "بالانس", render: (c) => <span dir="ltr" className="font-jbm-regular">{formatNumber(c.balance)}</span>, sortName: "Balance", direction: "text-left" },
        {
            key: 8,
            width: "w-[14%]",
            label: "عملیات",
            render: (c) => tableOperation(c.id, ["trades/details", "trades/update", "delete"], () => handleDeleteAlert(c)),
            direction: "text-left"
        },
    ];

    return (
        <div className="p-5 flex flex-col min-h-[616px]">
            <DeleteModal
                setIsModalOpen={setIsModalOpen}
                isModalOpen={isModalOpen}
                onDelete={handleDelete}
                message={`آیا از حذف معامله در ${formatDateTime(new Date(deleteTrade?.entryDateTime as string))} مطمئن هستید؟`}
                isLoading={deleteLoading}
            />
            <div className="py-6">
                <h1 className="text-white/80 font-iran-sans font-bold text-lg">معاملات</h1>
            </div>

            <Table
                    data={data}
                    config={config}
                    keyFn={(c) => c.id}
                    onPageClick={handlePageClick}
                    onSearch={handleSearch}
                    numberOfButton={5}
                    onHeaderSortClick={handleHeaderSortClick}
                    sorts={sorts}
                    search={search}
                    loading={isLoading}
                    searchPlaceholder={"جستجو در تاریخ ورود..."}
                    headerContent={
                        <TableAddBtn disabled={selectedPlan?.id === undefined} message={
                            <div className="flex justify-center items-center">
                                <SquarePlus strokeWidth={0.75} className="size-4 stroke-white/80 ml-2"/>
                                <span>افزودن</span>
                            </div>
                        }
                                     path={`trades/create/${selectedPlan?.id}`}
                        />
                    }
                />


        </div>
    );
};

export default Trades;
