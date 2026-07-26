import { Modal } from "../atoms/Modal";

interface Props {
  open: boolean;
  title: string;
  message: string;
  loading: boolean;
  onConfirm: () => void;
  onCancel: () => void;
}

export function ConfirmDelete({
  open,
  title,
  message,
  loading,
  onConfirm,
  onCancel,
}: Props) {
  return (
    <Modal open={open} title={title} onClose={onCancel}>
      <p className="text-sm text-gray-600">{message}</p>
      <div className="mt-6 flex justify-end gap-2">
        <button
          onClick={onCancel}
          disabled={loading}
          className="rounded-lg border border-gray-300 px-4 py-2 text-sm text-gray-700 hover:bg-gray-50"
        >
          Cancel
        </button>
        <button
          onClick={onConfirm}
          disabled={loading}
          className="rounded-lg bg-red-600 px-4 py-2 text-sm text-white hover:bg-red-700 disabled:opacity-50"
        >
          {loading ? "Deleting..." : "Delete"}
        </button>
      </div>
    </Modal>
  );
}
