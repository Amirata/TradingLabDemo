import {useEffect, useState, useCallback} from "react";
import {Link} from "react-router-dom";
import {SquarePlus} from "lucide-react";

import Table, {Config} from "../../ui/tables/Table";
import {SquareBtn, TableAddBtn} from "../../ui/my-buttons.tsx";
import apisWrapper from "../../../libs/apis-wrapper.ts";


import {
    Sort,
    PaginationDto,
    PaginatedResult,
    TablePlan, QueryDto,
} from "../../../libs/definitions.ts";
import {DeleteIcon, EditIcon, EyeIcon} from "../../icons.tsx";
import {DeleteModal} from "../../ui/modals/Modal.tsx";
import {useAppStore} from "../../../libs/stores/app-store.ts";


const usePlansData = (trigger: boolean) => {
    const [page, setPage] = useState<PaginationDto>({ pageSize: 5, pageNumber: 1 });
    const [search, setSearch] = useState<string>("");
    const [data, setData] = useState<PaginatedResult<TablePlan> | undefined>();
    const [isLoading, setIsLoading] = useState(true);
    const [sorts, setSorts] = useState<Sort[]>([]);

    useEffect(() => {
        setIsLoading(true);

        const queryDto: QueryDto = { ...page, search, sorts };

        apisWrapper.PlanWrapper.list(queryDto)
            .then((newData) => {
                setData(newData);
            })
            .catch(console.error)
            .finally(() => setIsLoading(false));
    }, [page, search, sorts, trigger]);

    return { data, isLoading, setPage, setSearch, setSorts, search, sorts, page };
};

const usePlanDelete = (
    setPage: (setter: (prev: PaginationDto) => PaginationDto) => void,
    currentPage: PaginationDto,
    dataLength: number,
    setUpdateTableData: (setter: (prev: boolean) => boolean) => void // اضافه کردن setUpdateTableData
) => {
    const [deletePlan, setDeletePlan] = useState<TablePlan | undefined>(undefined);
    const [deleteLoading, setDeleteLoading] = useState<boolean>(false);
    const [isModalOpen, setIsModalOpen] = useState<boolean>(false);
    const { setSelectedPlan } = useAppStore();

    const handleDeleteAlert = <T extends TablePlan>(obj: T) => {
        setDeletePlan(obj);
        setIsModalOpen(true);
        setSelectedPlan(null);
    };

    const handleDelete = useCallback(() => {
        if (!deletePlan) return;
        setDeleteLoading(true);
        apisWrapper.PlanWrapper.delete(deletePlan.id)
            .then(() => {
                if (dataLength === 1 && currentPage.pageNumber > 1) {
                    setPage((prev) => ({
                        ...prev,
                        pageNumber: prev.pageNumber - 1,
                    }));
                }
                setUpdateTableData((prev) => !prev); // استفاده از setUpdateTableData که از Plans اومده
            })
            .catch((error) => {
                console.error("Error deleting Plan:", error);
            })
            .finally(() => {
                setDeleteLoading(false);
                setIsModalOpen(false);
            });
    }, [deletePlan, setPage, currentPage, dataLength, setUpdateTableData]);

    return { deleteLoading, handleDeleteAlert, handleDelete, isModalOpen, setIsModalOpen, deletePlan };
};

const Plans = () => {
    const [updateTableData, setUpdateTableData] = useState<boolean>(false);

    const { data, isLoading, setPage, setSearch, setSorts, search, sorts, page } = usePlansData(updateTableData);

    const {
        deleteLoading,
        handleDeleteAlert,
        handleDelete,
        isModalOpen,
        setIsModalOpen,
        deletePlan,
    } = usePlanDelete(setPage, page, data?.data.length || 0, setUpdateTableData); // پاس دادن setUpdateTableData

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

    const tableOperation = (id: string | number, paths: string[], onDeleteFunc: <T>(obj: T) => void) => (
        <div className="flex items-center justify-end">
            {paths.map((path, index) => {
                if (path === "delete") {
                    return (
                        <SquareBtn key={index} message={<DeleteIcon />} onClick={onDeleteFunc} />
                    );
                } else {
                    return (
                        <Link key={index} to={`${path}/${id}`}>
                            <SquareBtn message={[<EyeIcon />, <EditIcon />][index]} />
                        </Link>
                    );
                }
            })}
        </div>
    );

    const config: Array<Config<TablePlan>> = [
        { key: 1, width: "w-[25%]", label: "نام", render: (c) => <span>{c.name}</span>, sortName: "Name" },
        { key: 2, width: "w-[10%]", label: "از ساعت", render: (c) => <span className="font-jbm-regular">{c.fromTime}</span>, sortName: "FromTime", direction: "text-left" },
        { key: 3, width: "w-[10%]", label: "تا ساعت", render: (c) => <span className="font-jbm-regular">{c.toTime}</span>, sortName: "ToTime", direction: "text-left" },
        { key: 4, width: "w-[40%]", label: "روزهای هفته", render: (c) => <span className="font-jbm-regular">{c.selectedDays.toString()}</span>, direction: "text-left" },
        {
            key: 5,
            width: "w-[15%]",
            label: "عملیات",
            render: (c) => tableOperation(c.id, ["details", "update", "delete"], () => handleDeleteAlert(c)),
            direction: "text-left"
        },
    ];

    return (
        <div className="p-5 flex flex-col min-h-[616px]">
            <DeleteModal
                setIsModalOpen={setIsModalOpen}
                isModalOpen={isModalOpen}
                onDelete={handleDelete}
                message={`آیا از حذف ${deletePlan?.name} مطمئن هستید؟`}
                isLoading={deleteLoading}
            />
            <div className="py-6">
                <h1 className="text-white/80 font-iran-sans font-bold text-lg">پلن ها</h1>
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
                searchPlaceholder={"جستجو در نام..."}
                headerContent={
                    <TableAddBtn
                        message={
                            <div className="flex justify-center items-center">
                                <SquarePlus strokeWidth={0.75} className="size-4 stroke-white/80 ml-2" />
                                <span>افزودن</span>
                            </div>
                        }
                        path="create"
                    />
                }
            />
        </div>
    );
};

export default Plans;



