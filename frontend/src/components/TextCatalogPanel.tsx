import { useEffect, useState } from 'react';
import { ApiError, formatApiError } from '../api/client';

export interface CatalogItem {
  id: number;
  nombre: string;
}

interface TextCatalogPanelProps {
  title: string;
  description: string;
  emptyHint: string;
  loadItems: () => Promise<CatalogItem[]>;
  createItem: (nombre: string) => Promise<CatalogItem>;
  updateItem: (id: number, nombre: string) => Promise<CatalogItem>;
  deleteItem: (id: number) => Promise<void>;
}

export function TextCatalogPanel({
  title,
  description,
  emptyHint,
  loadItems,
  createItem,
  updateItem,
  deleteItem
}: TextCatalogPanelProps) {
  const [items, setItems] = useState<CatalogItem[]>([]);
  const [nombre, setNombre] = useState('');
  const [editingId, setEditingId] = useState<number | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [feedback, setFeedback] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  async function reload() {
    setIsLoading(true);
    setError(null);

    try {
      setItems(await loadItems());
    } catch (caughtError) {
      setError(formatApiError(caughtError));
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    void reload();
  }, []);

  function startEdit(item: CatalogItem) {
    setEditingId(item.id);
    setNombre(item.nombre);
    setFeedback(null);
    setError(null);
  }

  function resetForm() {
    setEditingId(null);
    setNombre('');
  }

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setIsSaving(true);
    setError(null);
    setFeedback(null);

    try {
      const trimmedNombre = nombre.trim();
      const savedItem = editingId === null
        ? await createItem(trimmedNombre)
        : await updateItem(editingId, trimmedNombre);

      setItems((currentItems) =>
        editingId === null
          ? [savedItem, ...currentItems]
          : currentItems.map((item) => (item.id === savedItem.id ? savedItem : item))
      );
      setFeedback(editingId === null ? 'Elemento creado.' : 'Elemento actualizado.');
      resetForm();
    } catch (caughtError) {
      setError(formatApiError(caughtError));
    } finally {
      setIsSaving(false);
    }
  }

  async function handleDelete(id: number) {
    if (!window.confirm('¿Eliminar este elemento?')) {
      return;
    }

    setIsSaving(true);
    setError(null);
    setFeedback(null);

    try {
      await deleteItem(id);
      setItems((currentItems) => currentItems.filter((item) => item.id !== id));
      setFeedback('Elemento eliminado.');
      if (editingId === id) {
        resetForm();
      }
    } catch (caughtError) {
      if (caughtError instanceof ApiError && caughtError.status === 409) {
        setError(caughtError.message);
      } else {
        setError(formatApiError(caughtError));
      }
    } finally {
      setIsSaving(false);
    }
  }

  return (
    <section className="panel-card">
      <div className="panel-heading">
        <div>
          <p className="panel-kicker">Catalogo simple</p>
          <h2>{title}</h2>
          <p className="panel-description">{description}</p>
        </div>
      </div>

      <form className="stack-form" onSubmit={handleSubmit}>
        <label className="field">
          <span>Nombre</span>
          <input
            value={nombre}
            onChange={(event) => setNombre(event.target.value)}
            placeholder="Nombre visible"
            required
            maxLength={100}
          />
        </label>

        <div className="form-actions">
          <button className="primary-button" type="submit" disabled={isSaving}>
            {editingId === null ? 'Crear' : 'Guardar'}
          </button>
          {editingId !== null ? (
            <button className="secondary-button" type="button" onClick={resetForm}>
              Cancelar edicion
            </button>
          ) : null}
        </div>
      </form>

      {feedback ? <p className="notice success">{feedback}</p> : null}
      {error ? <p className="notice error">{error}</p> : null}

      <div className="list-block">
        <div className="list-header">
          <h3>Elementos</h3>
          <span>{items.length}</span>
        </div>

        {isLoading ? <p className="empty-state">Cargando...</p> : null}
        {!isLoading && items.length === 0 ? <p className="empty-state">{emptyHint}</p> : null}

        <div className="compact-list">
          {items.map((item) => (
            <article className="compact-row" key={item.id}>
              <div>
                <strong>{item.nombre}</strong>
                <p>ID {item.id}</p>
              </div>
              <div className="row-actions">
                <button type="button" className="secondary-button" onClick={() => startEdit(item)}>
                  Editar
                </button>
                <button type="button" className="danger-button" onClick={() => void handleDelete(item.id)} disabled={isSaving}>
                  Borrar
                </button>
              </div>
            </article>
          ))}
        </div>
      </div>
    </section>
  );
}