import { useQuery } from "@tanstack/react-query";
import { categoriesApi } from "./api";

export function useCategories() {
  return useQuery({
    queryKey: ["categories"],
    queryFn: () => categoriesApi.getAll(),
  });
}

export function useCategoriesDropdown() {
  return useQuery({
    queryKey: ["categories", "dropdown"],
    queryFn: () => categoriesApi.getDropdown(),
  });
}
