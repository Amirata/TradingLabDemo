import {Combobox, ComboboxButton, ComboboxInput, ComboboxOption, ComboboxOptions} from '@headlessui/react'
import {CheckIcon} from '@heroicons/react/20/solid'
import clsx from 'clsx'
import {useEffect, useState} from 'react'
import {ChevronDown} from "lucide-react";
import apisWrapper from '../../libs/apis-wrapper';
import {PaginationResult, SelectedPlan, TablePlan} from "../../libs/definitions.ts";
import {useAppStore} from "../../libs/stores/app-store.ts";

const people = [
    {id: 1, name: 'Tom Cook'},
    {id: 2, name: 'Wade Cooper'},
    {id: 3, name: 'Tanya Fox'},
    {id: 4, name: 'Arlene Mccoy'},
    {id: 5, name: 'Devon Webb'},
]

type person = {
    id: number,
    name: string
}

export const SimpleCombobox = () => {
    const [query, setQuery] = useState('')
    const [selected, setSelected] = useState<person|null>(people[1])

    const filteredPeople =
        query === ''
            ? people
            : people.filter((person) => {
                return person.name.toLowerCase().includes(query.toLowerCase())
            })

    return (
        <div className="w-52">
            <Combobox value={selected} onChange={(value) => setSelected(value)} onClose={() => setQuery('')}>
                <div className="relative">
                    <ComboboxInput
                        className={clsx(
                            'w-full rounded-sm border-none bg-zinc-700 py-1.5 pr-8 pl-3 text-sm/6 text-white',
                            'focus:outline-none data-[focus]:outline-2 data-[focus]:-outline-offset-2 data-[focus]:outline-white/25'
                        )}
                        displayValue={(person:person) => person?.name}
                        onChange={(event) => setQuery(event.target.value)}
                    />
                    <ComboboxButton className="group absolute inset-y-0 right-0 px-2.5">
                        <ChevronDown strokeWidth={0.75} className="size-4 stroke-zinc-100" />
                    </ComboboxButton>
                </div>

                <ComboboxOptions
                    anchor="bottom"
                    transition
                    className={clsx(
                        'w-[var(--input-width)] rounded-sm mt-1 border border-white/5 bg-zinc-100/20 p-1 [--anchor-gap:var(--spacing-1)] empty:invisible',
                        'transition duration-100 ease-in data-[leave]:data-[closed]:opacity-0'
                    )}
                >
                    {filteredPeople.map((person) => (
                        <ComboboxOption
                            key={person.id}
                            value={person}
                            className="group flex justify-between cursor-default items-center gap-2 rounded-sm py-1.5 px-3 select-none data-[focus]:bg-zinc-900/60"
                        >
                            <CheckIcon className="invisible size-4 fill-white group-data-[selected]:visible"/>
                            <div className="text-sm/6 text-white">{person.name}</div>
                        </ComboboxOption>
                    ))}
                </ComboboxOptions>
            </Combobox>
        </div>
    )
}




export const PlansCombobox = () => {
    const [plans, setPlans] = useState<SelectedPlan[]>([])
    const [query, setQuery] = useState('')
    const {selectedPlan, setSelectedPlan, setSelectedYear} = useAppStore();

    useEffect(() => {
        apisWrapper.requests
            .get<
                PaginationResult<TablePlan>
            >(`trading-plan?pageIndex=1&pageSize=100000`)
            .then((r) => {
                const planList =r.data.map(item => ({ id: item.id, name: item.name } as SelectedPlan))
                setPlans(planList);
                if (selectedPlan === null && planList.length > 0) {
                    setSelectedPlan(planList[0]);
                }
            });
    }, []);



    const filteredPlan =
        query === ''
            ? plans
            : plans.filter((plan) => {
                return plan.name.includes(query)
            })

    return (
        <div className="w-52">
            { selectedPlan && <Combobox disabled={plans.length == 0} value={selectedPlan} onChange={(value) => {setSelectedPlan(value); setSelectedYear(null)}} onClose={() => setQuery('')}>
                <div className="relative">
                    <ComboboxInput
                        className={clsx(
                            'w-full rounded-sm py-1.5 pr-2 border border-zinc-600 pl-3 text-sm/6 text-white font-iran-sans-fa-num',
                            'focus:bg-zinc-700 focus:outline-none data-[focus]:outline-2 data-[focus]:-outline-offset-2 data-[focus]:outline-white/25'
                        )}
                        displayValue={(plan:SelectedPlan) => plan?.name}
                        onChange={(event) => setQuery(event.target.value)}
                    />
                    <ComboboxButton className="group absolute inset-y-0 left-0 px-2.5">
                        <ChevronDown strokeWidth={0.75} className="size-4 stroke-white/80" />
                    </ComboboxButton>
                </div>

                <ComboboxOptions
                    anchor="bottom"
                    transition
                    className={clsx(
                        'w-[var(--input-width)] rounded-sm mt-1 border border-zinc-500 bg-zinc-700 p-1 [--anchor-gap:var(--spacing-1)] empty:invisible',
                        'transition duration-100 ease-in data-[leave]:data-[closed]:opacity-0'
                    )}
                >
                    {filteredPlan.map((plan) => (
                        <ComboboxOption
                            key={plan.id}
                            value={plan}
                            className="group flex justify-between cursor-default items-center gap-2 rounded-sm py-1.5 px-3 select-none data-[focus]:bg-zinc-900/60"
                        >
                            <CheckIcon className="invisible size-4 fill-white group-data-[selected]:visible"/>
                            <div className="text-sm/6 text-white font-iran-sans">{plan.name}</div>
                        </ComboboxOption>
                    ))}
                </ComboboxOptions>
            </Combobox>
            }
        </div>
    )
}