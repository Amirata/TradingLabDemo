import { create } from "zustand";
import {CalendarData, SelectedPlan} from "../definitions.ts";

type State = {
  selectedPlan: SelectedPlan | null;
  selectedYear: number | null;
  calendarData: CalendarData | null;
  tradeDeleted: boolean;
  symbolList: number[];
  selectedSymbol:number | null;
};

type Actions = {
  setSelectedPlan: (key: SelectedPlan | null) => void;
  setSelectedYear: (key: number | null) => void;
  setCalendarData: (key: CalendarData | null) => void;
  setSymbolList: (symbols: number[]) => void;
  setSelectedSymbol: (symbol: number | null) => void;
  setTradeDeleted: () => void;
  reset: () => void;
};

const initialState: State = {
  selectedPlan: null,
  selectedYear: null,
  calendarData: null,
  tradeDeleted: false,
  selectedSymbol: null,
  symbolList: []
};

export const useAppStore = create<State & Actions>((set) => ({
  ...initialState,

  setSelectedPlan: (plan: SelectedPlan | null) => {
    set(() => ({
      selectedPlan: plan,
    }));
  },
  setSelectedYear: (year: number | null) => {
    set(() => ({
      selectedYear: year,
    }));
  },
  setCalendarData: (calendar: CalendarData | null) => {
    set(() => ({
      calendarData: calendar,
    }));
  },
  setSymbolList: (symbols: number[]) => {
    set(() => ({
      symbolList: symbols,
    }));
  },
  setSelectedSymbol: (selectedSymbol: number|null) => {
    set(() => ({
      selectedSymbol: selectedSymbol,
    }));
  },
  setTradeDeleted: () => {
    set((state) => ({
      tradeDeleted: !state.tradeDeleted,
    }));
  },
  reset: () => set(initialState),
}));
