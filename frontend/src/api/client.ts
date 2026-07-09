export class ApiError extends Error {
  public readonly status: number;
  public readonly fields: Record<string, string[]> | null;

  constructor(message: string, status: number, fields: Record<string, string[]> | null = null) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.fields = fields;
  }
}

const apiBaseUrl = (import.meta.env.VITE_API_BASE_URL as string | undefined) ?? '/api';

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === 'object' && value !== null;
}

async function readError(response: Response): Promise<ApiError> {
  const contentType = response.headers.get('content-type') ?? '';
  let message = `Error ${response.status}`;
  let fields: Record<string, string[]> | null = null;

  if (contentType.includes('application/json')) {
    try {
      const payload: unknown = await response.json();

      if (isRecord(payload)) {
        if (typeof payload.mensaje === 'string' && payload.mensaje.trim().length > 0) {
          message = payload.mensaje;
        } else if (typeof payload.title === 'string' && payload.title.trim().length > 0) {
          message = payload.title;
        } else if (typeof payload.detail === 'string' && payload.detail.trim().length > 0) {
          message = payload.detail;
        }

        if (isRecord(payload.errors)) {
          fields = Object.fromEntries(
            Object.entries(payload.errors).map(([key, value]) => [
              key,
              Array.isArray(value) ? value.map((item) => String(item)) : [String(value)]
            ])
          );

          const fieldMessages = Object.values(fields).flat();
          if (fieldMessages.length > 0) {
            message = fieldMessages[0];
          }
        }
      }
    } catch {
      message = `Error ${response.status}`;
    }
  } else {
    try {
      const text = await response.text();
      if (text.trim().length > 0) {
        message = text.trim();
      }
    } catch {
      message = `Error ${response.status}`;
    }
  }

  return new ApiError(message, response.status, fields);
}

export async function apiRequest<T>(path: string, init: RequestInit = {}): Promise<T> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    ...init,
    headers: {
      Accept: 'application/json',
      ...(init.body !== undefined ? { 'Content-Type': 'application/json' } : {}),
      ...(init.headers ?? {})
    }
  });

  if (!response.ok) {
    throw await readError(response);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  const contentType = response.headers.get('content-type') ?? '';
  if (contentType.includes('application/json')) {
    return (await response.json()) as T;
  }

  return (await response.text()) as T;
}

export function formatApiError(error: unknown): string {
  if (error instanceof ApiError) {
    return error.message;
  }

  if (error instanceof Error && error.message.trim().length > 0) {
    return error.message;
  }

  return 'No se ha podido completar la peticion.';
}