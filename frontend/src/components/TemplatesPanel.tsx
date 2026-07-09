import { useEffect, useMemo, useState } from 'react';
import { formatApiError } from '../api/client';
import {
  createPlantilla,
  createTaskFromTemplate,
  deletePlantilla,
  getPlantillas,
  updatePlantilla
} from '../api/resources';
import type { PlantillaTareaDto, TipoRecurrencia } from '../api/contracts';

interface TemplateFormState {
  titulo: string;
  notas: string;
  esRepetitiva: boolean;
  tipoRecurrencia: TipoRecurrencia | '';
  categoriaId: string;
  estaActiva: boolean;
}

const initialFormState: TemplateFormState = {
  titulo: '',
  notas: '',
  esRepetitiva: false,
  tipoRecurrencia: '',
  categoriaId: '',
  estaActiva: true
};

const recurrenciaLabels: Record<number, string> = {
  1: 'Diaria',
  2: 'Semanal',
  3: 'Mensual'
};

export function TemplatesPanel() {
  const [items, setItems] = useState<PlantillaTareaDto[]>([]);
  const [form, setForm] = useState<TemplateFormState>(initialFormState);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [feedback, setFeedback] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  async function reload() {
    setIsLoading(true);
    setError(null);

    try {
      setItems(await getPlantillas());
    } catch (caughtError) {
      setError(formatApiError(caughtError));
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    void reload();
  }, []);

  const totalActivas = useMemo(() => items.filter((item) => item.estaActiva).length, [items]);

  function startEdit(item: PlantillaTareaDto) {
    setEditingId(item.id);
    setForm({
      titulo: item.titulo,
      notas: item.notas ?? '',
      esRepetitiva: item.esRepetitiva,
      tipoRecurrencia: item.tipoRecurrencia ?? '',
      categoriaId: item.categoriaId !== null ? String(item.categoriaId) : '',
      estaActiva: item.estaActiva
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
        titulo: form.titulo.trim(),
        notas: form.notas.trim().length > 0 ? form.notas.trim() : null,
        esRepetitiva: form.esRepetitiva,
        tipoRecurrencia: form.esRepetitiva && form.tipoRecurrencia !== '' ? form.tipoRecurrencia : null,
        categoriaId: form.categoriaId.trim().length > 0 ? Number(form.categoriaId) : null,
        estaActiva: form.estaActiva
      };

      const savedItem = editingId === null
        ? await createPlantilla(payload)
        : await updatePlantilla(editingId, payload);

      setItems((currentItems) =>
        editingId === null
          ? [savedItem, ...currentItems]
          : currentItems.map((item) => (item.id === savedItem.id ? savedItem : item))
      );
      setFeedback(editingId === null ? 'Plantilla creada.' : 'Plantilla actualizada.');
      resetForm();
    } catch (caughtError) {
      setError(formatApiError(caughtError));
    } finally {
      setIsSaving(false);
    }
  }

  async function handleDelete(id: number) {
    if (!window.confirm('¿Eliminar esta plantilla?')) {
      return;
    }

    setIsSaving(true);
    setError(null);
    setFeedback(null);

    try {
      await deletePlantilla(id);
      setItems((currentItems) => currentItems.filter((item) => item.id !== id));
      setFeedback('Plantilla eliminada.');
      if (editingId === id) {
        resetForm();
      }
    } catch (caughtError) {
      setError(formatApiError(caughtError));
    } finally {
      setIsSaving(false);
    }
  }

  async function handleInstantiate(id: number) {
    setIsSaving(true);
    setError(null);
    setFeedback(null);

    try {
      const createdTask = await createTaskFromTemplate(id);
      setFeedback(`Tarea creada desde la plantilla con ID ${createdTask.id}.`);
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
          <p className="panel-kicker">Recursos reutilizables</p>
          <h2>Plantillas</h2>
          <p className="panel-description">CRUD simple e instancia de tarea desde plantilla existente.</p>
        </div>
        <div className="panel-metrics">
          <div>
            <strong>{items.length}</strong>
            <span>Total</span>
          </div>
          <div>
            <strong>{totalActivas}</strong>
            <span>Activas</span>
          </div>
        </div>
      </div>

      <form className="stack-form" onSubmit={handleSubmit}>
        <label className="field">
          <span>Titulo</span>
          <input
            value={form.titulo}
            onChange={(event) => setForm((current) => ({ ...current, titulo: event.target.value }))}
            placeholder="Plantilla de seguimiento"
            required
            maxLength={200}
          />
        </label>

        <label className="field">
          <span>Notas</span>
          <textarea
            value={form.notas}
            onChange={(event) => setForm((current) => ({ ...current, notas: event.target.value }))}
            rows={3}
          />
        </label>

        <div className="two-column-form">
          <label className="field field-inline">
            <input
              type="checkbox"
              checked={form.esRepetitiva}
              onChange={(event) =>
                setForm((current) => ({
                  ...current,
                  esRepetitiva: event.target.checked,
                  tipoRecurrencia: event.target.checked ? current.tipoRecurrencia || 1 : ''
                }))
              }
            />
            <span>Es repetitiva</span>
          </label>

          <label className="field field-inline">
            <input
              type="checkbox"
              checked={form.estaActiva}
              onChange={(event) => setForm((current) => ({ ...current, estaActiva: event.target.checked }))}
            />
            <span>Activa</span>
          </label>
        </div>

        <div className="two-column-form">
          <label className="field">
            <span>Tipo de recurrencia</span>
            <select
              value={form.tipoRecurrencia}
              onChange={(event) =>
                setForm((current) => ({ ...current, tipoRecurrencia: event.target.value === '' ? '' : Number(event.target.value) as TipoRecurrencia }))
              }
              disabled={!form.esRepetitiva}
            >
              <option value="">No aplica</option>
              {Object.entries(recurrenciaLabels).map(([value, label]) => (
                <option key={value} value={value}>
                  {label}
                </option>
              ))}
            </select>
          </label>

          <label className="field">
            <span>Categoria ID opcional</span>
            <input
              type="number"
              min="1"
              value={form.categoriaId}
              onChange={(event) => setForm((current) => ({ ...current, categoriaId: event.target.value }))}
              placeholder="Solo si existe en backend"
            />
          </label>
        </div>

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
          <h3>Plantillas registradas</h3>
          <span>{items.length}</span>
        </div>

        {isLoading ? <p className="empty-state">Cargando...</p> : null}
        {!isLoading && items.length === 0 ? <p className="empty-state">No hay plantillas todavia.</p> : null}

        <div className="entity-grid">
          {items.map((item) => (
            <article className="entity-card" key={item.id}>
              <div className="entity-title-row">
                <div>
                  <strong>{item.titulo}</strong>
                  <p>ID {item.id}</p>
                </div>
                <span className={item.estaActiva ? 'status-pill status-ok' : 'status-pill'}>
                  {item.estaActiva ? 'Activa' : 'Inactiva'}
                </span>
              </div>

              <dl className="details-list">
                <div>
                  <dt>Recurrencia</dt>
                  <dd>{item.esRepetitiva ? recurrenciaLabels[item.tipoRecurrencia ?? 1] ?? 'Repetitiva' : 'No repetitiva'}</dd>
                </div>
                <div>
                  <dt>Categoria</dt>
                  <dd>{item.categoriaId ?? 'Sin asignar'}</dd>
                </div>
              </dl>

              {item.notas ? <p className="muted-text">{item.notas}</p> : null}

              <div className="row-actions">
                <button type="button" className="secondary-button" onClick={() => startEdit(item)}>
                  Editar
                </button>
                <button type="button" className="secondary-button" onClick={() => void handleInstantiate(item.id)} disabled={isSaving}>
                  Instanciar
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