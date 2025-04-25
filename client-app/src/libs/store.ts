import { create } from "zustand";
import { combine } from "zustand/middleware";


export const useGameStore = create(
  combine(
    {
      history: [Array(9).fill(null)],
      currentMove: 0,
    },
    (set) => {
      return {
        setHistory: (nextHistory: any[] | ((a: any[]) => any[])) => {
          set((state) => ({
            history:
              typeof nextHistory === "function"
                ? nextHistory(state.history)
                : nextHistory,
          }));
        },
        setCurrentMove: (nextCurrentMove: number | ((i: number) => number)) => {
          set((state) => ({
            currentMove:
              typeof nextCurrentMove === "function"
                ? nextCurrentMove(state.currentMove)
                : nextCurrentMove,
          }));
        },
      };
    },
  ),
);
