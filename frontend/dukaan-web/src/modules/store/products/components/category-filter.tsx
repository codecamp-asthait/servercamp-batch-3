import type { StorefrontCategory } from "../types";

interface CategoryFilterProps {
  categories: StorefrontCategory[];
  activeId: string | null;
  onSelect: (id: string | null) => void;
}

function CategoryNode({
  category,
  activeId,
  onSelect,
  depth = 0,
}: {
  category: StorefrontCategory;
  activeId: string | null;
  onSelect: (id: string | null) => void;
  depth?: number;
}) {
  const isActive = activeId === category.id;

  return (
    <li>
      <button
        onClick={() => onSelect(category.id)}
        style={{ paddingLeft: `${0.75 + depth * 1}rem` }}
        className={`w-full text-left py-1.5 pr-3 rounded-lg text-sm transition-colors ${
          isActive ? "bg-zinc-900 text-white font-medium" : "text-zinc-600 hover:bg-zinc-100"
        }`}
      >
        {depth > 0 && <span className="mr-1 text-zinc-400">↳</span>}
        {category.name}
      </button>
      {category.subCategories.length > 0 && (
        <ul className="flex flex-col gap-0.5 mt-0.5">
          {category.subCategories.map((sub) => (
            <CategoryNode
              key={sub.id}
              category={sub}
              activeId={activeId}
              onSelect={onSelect}
              depth={depth + 1}
            />
          ))}
        </ul>
      )}
    </li>
  );
}

export function CategoryFilter({ categories, activeId, onSelect }: CategoryFilterProps) {
  const topLevel = categories.filter((c) => c.parentCategoryId === null);

  return (
    <nav>
      <ul className="flex flex-col gap-0.5">
        <li>
          <button
            onClick={() => onSelect(null)}
            className={`w-full text-left px-3 py-1.5 rounded-lg text-sm transition-colors ${
              activeId === null ? "bg-zinc-900 text-white font-medium" : "text-zinc-600 hover:bg-zinc-100"
            }`}
          >
            All
          </button>
        </li>
        {topLevel.map((cat) => (
          <CategoryNode key={cat.id} category={cat} activeId={activeId} onSelect={onSelect} />
        ))}
      </ul>
    </nav>
  );
}
