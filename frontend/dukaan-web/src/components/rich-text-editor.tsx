"use client";

import { useEditor, EditorContent } from "@tiptap/react";
import StarterKit from "@tiptap/starter-kit";
import { Bold, Italic, List, ListOrdered } from "lucide-react";

interface RichTextEditorProps {
  value?: string;
  onChange: (html: string) => void;
}

function ToolbarButton({
  onClick,
  active,
  children,
  title,
}: {
  onClick: () => void;
  active?: boolean;
  children: React.ReactNode;
  title: string;
}) {
  return (
    <button
      type="button"
      onClick={onClick}
      title={title}
      className={`rounded p-1.5 transition-colors ${
        active ? "bg-zinc-200 text-zinc-900" : "text-zinc-400 hover:bg-zinc-100 hover:text-zinc-700"
      }`}
    >
      {children}
    </button>
  );
}

export function RichTextEditor({ value, onChange }: RichTextEditorProps) {
  const editor = useEditor({
    extensions: [StarterKit],
    content: value ?? "",
    immediatelyRender: false,
    onUpdate: ({ editor }) => onChange(editor.getHTML()),
  });

  return (
    <div className="rounded-md border border-zinc-200 overflow-hidden focus-within:ring-2 focus-within:ring-zinc-300">
      <div className="flex gap-0.5 border-b border-zinc-200 bg-zinc-50 px-2 py-1.5">
        <ToolbarButton
          onClick={() => editor?.chain().focus().toggleBold().run()}
          active={editor?.isActive("bold")}
          title="Bold"
        >
          <Bold size={14} />
        </ToolbarButton>
        <ToolbarButton
          onClick={() => editor?.chain().focus().toggleItalic().run()}
          active={editor?.isActive("italic")}
          title="Italic"
        >
          <Italic size={14} />
        </ToolbarButton>
        <ToolbarButton
          onClick={() => editor?.chain().focus().toggleBulletList().run()}
          active={editor?.isActive("bulletList")}
          title="Bullet list"
        >
          <List size={14} />
        </ToolbarButton>
        <ToolbarButton
          onClick={() => editor?.chain().focus().toggleOrderedList().run()}
          active={editor?.isActive("orderedList")}
          title="Ordered list"
        >
          <ListOrdered size={14} />
        </ToolbarButton>
      </div>
      <EditorContent
        editor={editor}
        className="px-3 py-2 text-sm outline-none [&_.ProseMirror]:outline-none [&_.ProseMirror]:min-h-[120px] [&_.ProseMirror_ul]:list-disc [&_.ProseMirror_ul]:pl-4 [&_.ProseMirror_ol]:list-decimal [&_.ProseMirror_ol]:pl-4"
      />
    </div>
  );
}
