interface Props {
  message: string;
  onRetry?: () => void;
}

export function ErrorAlert({ message, onRetry }: Props) {
  return (
    <div className="rounded-lg border border-red-200 bg-red-50 p-4 text-sm text-red-700">
      <p className="font-medium">Something went wrong</p>
      <p className="mt-1">{message}</p>
      {onRetry && (
        <button
          onClick={onRetry}
          className="mt-2 underline hover:no-underline"
        >
          Try again
        </button>
      )}
    </div>
  );
}
