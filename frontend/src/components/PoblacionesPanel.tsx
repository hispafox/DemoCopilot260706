import { useEffect, useState } from 'react';
import { ApiError, formatApiError } from '../api/client';
import { createPoblacion, deletePoblacion, getPoblaciones, updatePoblacion } from '../api/resources';
import type { PoblacionDto } from '../api/contracts';

interface PoblacionFormState {
  nombre: string;
  codigoIsoPais: string;
}

const initialFormState: PoblacionFormState = {
  nombre: '',
  codigoIsoPais: ''
};

export function PoblacionesPanel() {
  const [items, setItems] = useState<PoblacionDto[]>([]);
  const [form, setForm] = useState<PoblacionFormState>(initialFormState);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [feedback, setFeedback] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  async function reload() {
    setIsLoading(true);
    setError(null);

    try {
      setItems(await getPoblaciones());
    } catch (caughtError) {
      setError(formatApiError(caughtError));
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    void reload();
  }, []);

  function startEdit(item: PoblacionDto) {
    setEditingId(item.id);
    setForm({
      nombre: item.nombre,
      codigoIsoPais: item.codigoIsoPais
    });
    setFeedback(null);
    setError(null);
  }

  function resetForm() {
    setEditingId(null);
    setForm(initialFormState);
  }

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setIsSaving(true);
    setError(null);
    setFeedback(null);

    try {
      const payload = {
        nombre: form.nombre.trim(),
        codigoIsoPais: form.codigoIsoPais.trim().toUpperCase()
      };

      const savedItem = editingId === null
        ? await createPoblacion(payload)
        : await updatePoblacion(editingId, payload);

      setItems((currentItems) =>
        editingId === null
          ? [savedItem, ...currentItems]
          : currentItems.map((item) => (item.id === savedItem.id ? savedItem : item))
      );

      setFeedback(editingId === null ? 'Poblacion creada.' : 'Poblacion actualizada.');
      resetForm();
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

  async function handleDelete(id: number) {
    if (!window.confirm('¿Eliminar esta poblacion?')) {
      return;
    }

    setIsSaving(true);
    setError(null);
    setFeedback(null);

    try {
      await deletePoblacion(id);
      setItems((currentItems) => currentItems.filter((item) => item.id !== id));
      setFeedback('Poblacion eliminada.');
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
          <h2>Poblaciones</h2>
          <p className="panel-description">Gestion de poblaciones con codigo ISO de pais (ISO 3166-1 alpha-2).</p>
        </div>
      </div>

      <form className="stack-form" onSubmit={handleSubmit}>
        <label className="field">
          <span>Nombre</span>
          <input
            value={form.nombre}
            onChange={(event) => setForm((current) => ({ ...current, nombre: event.target.value }))}
            placeholder="Nombre visible"
            required
            maxLength={100}
          />
        </label>

        <label className="field">
          <span>Codigo ISO pais</span>
          <input
            value={form.codigoIsoPais}
            onChange={(event) => setForm((current) => ({ ...current, codigoIsoPais: event.target.value }))}
            placeholder="ES"
            required
            minLength={2}
            maxLength={2}
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
        {!isLoading && items.length === 0 ? <p className="empty-state">No hay poblaciones todavia.</p> : null}

        <div className="compact-list">
          {items.map((item) => (
            <article className="compact-row" key={item.id}>
              <div>
                <strong>{item.nombre}</strong>
                <p>ID {item.id} · ISO {item.codigoIsoPais}</p>
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
