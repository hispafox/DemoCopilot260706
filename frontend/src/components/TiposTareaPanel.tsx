import { useEffect, useState } from 'react';
import { formatApiError } from '../api/client';
import {
  createTipoTarea,
  deleteTipoTarea,
  getTiposTarea,
  updateTipoTarea
} from '../api/resources';
import type { TipoTareaDto } from '../api/contracts';

interface TipoTareaFormState {
  nombre: string;
  descripcion: string;
  estaActivo: boolean;
}

const initialFormState: TipoTareaFormState = {
  nombre: '',
  descripcion: '',
  estaActivo: true
};

export function TiposTareaPanel() {
  const [items, setItems] = useState<TipoTareaDto[]>([]);
  const [form, setForm] = useState<TipoTareaFormState>(initialFormState);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [feedback, setFeedback] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  async function reload() {
    setIsLoading(true);
    setError(null);

    try {
      setItems(await getTiposTarea());
    } catch (caughtError) {
      setError(formatApiError(caughtError));
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    void reload();
  }, []);

  function startEdit(item: TipoTareaDto) {
    setEditingId(item.id);
    setForm({
      nombre: item.nombre,
      descripcion: item.descripcion ?? '',
      estaActivo: item.estaActivo
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
        descripcion: form.descripcion.trim().length > 0 ? form.descripcion.trim() : null,
        estaActivo: form.estaActivo
      };

      const savedItem = editingId === null
        ? await createTipoTarea(payload)
        : await updateTipoTarea(editingId, payload);

      setItems((currentItems) =>
        editingId === null
          ? [savedItem, ...currentItems]
          : currentItems.map((item) => (item.id === savedItem.id ? savedItem : item))
      );
      setFeedback(editingId === null ? 'Tipo de tarea creado.' : 'Tipo de tarea actualizado.');
      resetForm();
    } catch (caughtError) {
      setError(formatApiError(caughtError));
    } finally {
      setIsSaving(false);
    }
  }

  async function handleDelete(id: number) {
    if (!window.confirm('¿Eliminar este tipo de tarea?')) {
      return;
    }

    setIsSaving(true);
    setError(null);
    setFeedback(null);

    try {
      await deleteTipoTarea(id);
      setItems((currentItems) => currentItems.filter((item) => item.id !== id));
      setFeedback('Tipo de tarea eliminado.');
      if (editingId === id) {
        resetForm();
      }
    } catch (caughtError) {
      setError(formatApiError(caughtError));
    } finally {
      setIsSaving(false);
    }
  }

  return (
    <section className="panel-card">
      <div className="panel-heading">
        <div>
          <p className="panel-kicker">Catalogo tecnico</p>
          <h2>Tipos de tarea</h2>
          <p className="panel-description">Usa este catalogo para alimentar el selector obligatorio de tareas.</p>
        </div>
      </div>

      <form className="stack-form" onSubmit={handleSubmit}>
        <div className="two-column-form">
          <label className="field">
            <span>Nombre</span>
            <input
              value={form.nombre}
              onChange={(event) => setForm((current) => ({ ...current, nombre: event.target.value }))}
              placeholder="Tipo de tarea"
              required
              maxLength={100}
            />
          </label>

          <label className="field field-inline">
            <input
              type="checkbox"
              checked={form.estaActivo}
              onChange={(event) => setForm((current) => ({ ...current, estaActivo: event.target.checked }))}
            />
            <span>Activo</span>
          </label>
        </div>

        <label className="field">
          <span>Descripcion</span>
          <textarea
            value={form.descripcion}
            onChange={(event) => setForm((current) => ({ ...current, descripcion: event.target.value }))}
            rows={3}
            maxLength={300}
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
        {!isLoading && items.length === 0 ? <p className="empty-state">No hay tipos de tarea aun.</p> : null}

        <div className="compact-list">
          {items.map((item) => (
            <article className="compact-row" key={item.id}>
              <div>
                <strong>{item.nombre}</strong>
                <p>
                  ID {item.id} {item.estaActivo ? '· Activo' : '· Inactivo'}
                </p>
                {item.descripcion ? <p>{item.descripcion}</p> : null}
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