"use client";

import { useState, useRef, useEffect } from "react";
import { ChevronDownIcon, ChevronUpIcon, SearchIcon, XIcon } from "lucide-react";

export interface MultiSelectOption {
  value: string;
  label: string; // may contain " > " path separators from backend
}

interface MultiSelectProps {
  options: MultiSelectOption[];
  value: string[];
  onChange: (value: string[]) => void;
  placeholder?: string;
}

function OptionLabel({ label }: { label: string }) {
  const parts = label.split(" > ");
  if (parts.length === 1) return <span className="font-medium text-zinc-800">{label}</span>;
  const prefix = parts.slice(0, -1).join(" / ");
  const name = parts[parts.length - 1];
  return (
    <span>
      <span className="text-zinc-400 text-xs">{prefix} / </span>
      <span className="font-medium text-zinc-800">{name}</span>
    </span>
  );
}

function leafName(label: string) {
  const parts = label.split(" > ");
  return parts[parts.length - 1];
}

export function MultiSelect({ options, value, onChange, placeholder = "Select…" }: MultiSelectProps) {
  const [open, setOpen] = useState(false);
  const [search, setSearch] = useState("");
  const ref = useRef<HTMLDivElement>(null);

  useEffect(() => {
    function handleClickOutside(e: MouseEvent) {
      if (ref.current && !ref.current.contains(e.target as Node)) setOpen(false);
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  function toggle(id: string) {
    onChange(value.includes(id) ? value.filter((v) => v !== id) : [...value, id]);
  }

  function remove(e: React.MouseEvent, id: string) {
    e.stopPropagation();
    onChange(value.filter((v) => v !== id));
  }

  function clearAll(e: React.MouseEvent) {
    e.stopPropagation();
    onChange([]);
  }

  const selected = options.filter((o) => value.includes(o.value));
  const filtered = options.filter((o) => o.label.toLowerCase().includes(search.toLowerCase()));

  return (
    <div ref={ref} className="relative">
      <div
        onClick={() => setOpen((v) => !v)}
        className="flex min-h-[38px] w-full cursor-pointer items-center gap-1.5 flex-wrap rounded-md border border-zinc-200 px-3 py-2 text-sm focus-within:ring-2 focus-within:ring-zinc-300"
      >
        {selected.length === 0 ? (
          <span className="text-zinc-400 flex-1">{placeholder}</span>
        ) : (
          <div className="flex flex-wrap gap-1.5 flex-1">
            {selected.map((opt) => (
              <span
                key={opt.value}
                className="flex items-center gap-1 rounded-md border border-zinc-200 bg-white px-2 py-0.5 text-xs text-zinc-700"
              >
                {leafName(opt.label)}
                <button
                  type="button"
                  onClick={(e) => remove(e, opt.value)}
                  className="text-zinc-400 hover:text-zinc-600"
                >
                  <XIcon className="size-3" />
                </button>
              </span>
            ))}
          </div>
        )}

        <div className="flex items-center gap-1 ml-auto shrink-0">
          {selected.length > 0 && (
            <button type="button" onClick={clearAll} className="text-zinc-400 hover:text-zinc-600">
              <XIcon className="size-4" />
            </button>
          )}
          {open
            ? <ChevronUpIcon className="size-4 text-zinc-500" />
            : <ChevronDownIcon className="size-4 text-zinc-500" />}
        </div>
      </div>

      {open && (
        <div className="absolute z-50 mt-1 w-full rounded-md border border-zinc-200 bg-white shadow-lg">
          <div className="flex items-center gap-2 border-b border-zinc-100 px-3 py-2">
            <SearchIcon className="size-4 shrink-0 text-zinc-400" />
            <input
              autoFocus
              type="text"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Search categories..."
              className="flex-1 text-sm outline-none placeholder:text-zinc-400"
            />
          </div>

          <ul className="max-h-60 overflow-y-auto py-1">
            {filtered.length === 0 ? (
              <li className="px-3 py-2 text-sm text-zinc-400">No results</li>
            ) : (
              filtered.map((opt) => (
                <li
                  key={opt.value}
                  onClick={() => toggle(opt.value)}
                  className="flex cursor-pointer items-center gap-3 px-3 py-2 hover:bg-zinc-50"
                >
                  <input
                    type="checkbox"
                    readOnly
                    checked={value.includes(opt.value)}
                    className="size-4 rounded border-zinc-300 accent-zinc-800"
                  />
                  <OptionLabel label={opt.label} />
                </li>
              ))
            )}
          </ul>
        </div>
      )}
    </div>
  );
}
