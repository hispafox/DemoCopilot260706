import { useEffect, useMemo, useState } from 'react';
import { ApiError, formatApiError } from '../api/client';
import {
  completeTarea,
  createTarea,
  deleteTarea,
  getPlantillas,
  getTareas,
  getTiposTarea,
  getUsuarios,
  updateTarea
} from '../api/resources';
import type {
  PlantillaTareaDto,
  PrioridadTarea,
  TareaDto,
  TipoTareaDto,
  TipoRecurrencia,
  UsuarioDto
} from '../api/contracts';

interface TaskFormState {
  titulo: string;
  estaCompletada: boolean;
  fechaVencimiento: string;
  notas: string;
  prioridad: PrioridadTarea;
  esRepetitiva: boolean;
  tipoRecurrencia: TipoRecurrencia | '';
  proximaRecurrencia: string;
  plantillaTareaId: string;
  categoriaId: string;
  usuarioId: string;
  tipoTareaId: string;
}

const initialFormState: TaskFormState = {
  titulo: '',
  estaCompletada: false,
  fechaVencimiento: '',
  notas: '',
  prioridad: 2,
  esRepetitiva: false,
  tipoRecurrencia: '',
  proximaRecurrencia: '',
  plantillaTareaId: '',
  categoriaId: '',
  usuarioId: '',
  tipoTareaId: ''
};

const prioridadLabels: Record<number, string> = {
  1: 'Baja',
  2: 'Normal',
  3: 'Alta',
  4: 'Urgente'
};

const recurrenciaLabels: Record<number, string> = {
  1: 'Diaria',
  2: 'Semanal',
  3: 'Mensual'
};

function toDateTimeLocalValue(value: string | null): string {
  if (!value) {
    return '';
  }

  const date = new Date(value);
  if (Number.isNaN(date.getTime())) {
    return '';
  }

  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  const hours = String(date.getHours()).padStart(2, '0');
  const minutes = String(date.getMinutes()).padStart(2, '0');

  return `${year}-${month}-${day}T${hours}:${minutes}`;
}

function fromDateTimeLocalValue(value: string): string | null {
  if (value.trim().length === 0) {
    return null;
  }

  const date = new Date(value);
  if (Number.isNaN(date.getTime())) {
    return null;
  }

  return date.toISOString();
}

function taskToFormState(task: TareaDto): TaskFormState {
  return {
    titulo: task.titulo,
    estaCompletada: task.estaCompletada,
    fechaVencimiento: toDateTimeLocalValue(task.fechaVencimiento),
    notas: task.notas ?? '',
    prioridad: task.prioridad,
    esRepetitiva: task.esRepetitiva,
    tipoRecurrencia: task.tipoRecurrencia ?? '',
    proximaRecurrencia: toDateTimeLocalValue(task.proximaRecurrencia),
    plantillaTareaId: task.plantillaTareaId !== null ? String(task.plantillaTareaId) : '',
    categoriaId: task.categoriaId !== null ? String(task.categoriaId) : '',
    usuarioId: task.usuarioId !== null ? String(task.usuarioId) : '',
    tipoTareaId: String(task.tipoTareaId)
  };
}

export function TasksPanel() {
  const [tasks, setTasks] = useState<TareaDto[]>([]);
  const [tiposTarea, setTiposTarea] = useState<TipoTareaDto[]>([]);
  const [usuarios, setUsuarios] = useState<UsuarioDto[]>([]);
  const [plantillas, setPlantillas] = useState<PlantillaTareaDto[]>([]);
  const [form, setForm] = useState<TaskFormState>(initialFormState);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [feedback, setFeedback] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  async function reload() {
    setIsLoading(true);
    setError(null);

    try {
      const [loadedTasks, loadedTipos, loadedUsuarios, loadedPlantillas] = await Promise.all([
        getTareas(),
        getTiposTarea(),
        getUsuarios(),
        getPlantillas()
      ]);

      setTasks(loadedTasks);
      setTiposTarea(loadedTipos);
      setUsuarios(loadedUsuarios);
      setPlantillas(loadedPlantillas);
    } catch (caughtError) {
      setError(formatApiError(caughtError));
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    void reload();
  }, []);

  const typeOptionsReady = useMemo(() => tiposTarea.length > 0, [tiposTarea.length]);

  function startEdit(task: TareaDto) {
    setEditingId(task.id);
    setForm(taskToFormState(task));
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
        estaCompletada: form.estaCompletada,
        fechaVencimiento: fromDateTimeLocalValue(form.fechaVencimiento),
        notas: form.notas.trim().length > 0 ? form.notas.trim() : null,
        prioridad: form.prioridad,
        esRepetitiva: form.esRepetitiva,
        tipoRecurrencia: form.esRepetitiva && form.tipoRecurrencia !== '' ? form.tipoRecurrencia : null,
        proximaRecurrencia: form.esRepetitiva ? fromDateTimeLocalValue(form.proximaRecurrencia) : null,
        plantillaTareaId: form.plantillaTareaId.trim().length > 0 ? Number(form.plantillaTareaId) : null,
        categoriaId: form.categoriaId.trim().length > 0 ? Number(form.categoriaId) : null,
        usuarioId: form.usuarioId.trim().length > 0 ? Number(form.usuarioId) : null,
        tipoTareaId: Number(form.tipoTareaId)
      };

      const savedTask = editingId === null
        ? await createTarea(payload)
        : await updateTarea(editingId, payload);

      setTasks((currentTasks) =>
        editingId === null
          ? [savedTask, ...currentTasks]
          : currentTasks.map((task) => (task.id === savedTask.id ? savedTask : task))
      );
      setFeedback(editingId === null ? 'Tarea creada.' : 'Tarea actualizada.');
      resetForm();
    } catch (caughtError) {
      setError(formatApiError(caughtError));
    } finally {
      setIsSaving(false);
    }
  }

  async function handleComplete(id: number) {
    setIsSaving(true);
    setError(null);
    setFeedback(null);

    try {
      const completedTask = await completeTarea(id);
      setTasks((currentTasks) => currentTasks.map((task) => (task.id === completedTask.id ? completedTask : task)));
      setFeedback(`Tarea ${id} marcada como completada.`);
    } catch (caughtError) {
      setError(formatApiError(caughtError));
    } finally {
      setIsSaving(false);
    }
  }

  async function handleDelete(id: number) {
    if (!window.confirm('¿Eliminar esta tarea?')) {
      return;
    }

    setIsSaving(true);
    setError(null);
    setFeedback(null);

    try {
      await deleteTarea(id);
      setTasks((currentTasks) => currentTasks.filter((task) => task.id !== id));
      setFeedback('Tarea eliminada.');
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
    <section className="panel-card panel-card-wide">
      <div className="panel-heading">
        <div>
          <p className="panel-kicker">Flujo principal</p>
          <h2>Tareas</h2>
          <p className="panel-description">Listado, alta, edicion, eliminacion y completado contra la API existente.</p>
        </div>
        <div className="panel-metrics">
          <div>
            <strong>{tasks.length}</strong>
            <span>Total</span>
          </div>
          <div>
            <strong>{tasks.filter((task) => task.estaCompletada).length}</strong>
            <span>Completadas</span>
          </div>
        </div>
      </div>

      <form className="stack-form" onSubmit={handleSubmit}>
        <label className="field">
          <span>Titulo</span>
          <input
            value={form.titulo}
            onChange={(event) => setForm((current) => ({ ...current, titulo: event.target.value }))}
            placeholder="Planificar demo"
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

        <div className="three-column-form">
          <label className="field">
            <span>Tipo de tarea</span>
            <select
              value={form.tipoTareaId}
              onChange={(event) => setForm((current) => ({ ...current, tipoTareaId: event.target.value }))}
              required
            >
              <option value="">Selecciona...</option>
              {tiposTarea.map((item) => (
                <option key={item.id} value={item.id}>
                  {item.nombre}
                </option>
              ))}
            </select>
          </label>

          <label className="field">
            <span>Prioridad</span>
            <select
              value={form.prioridad}
              onChange={(event) => setForm((current) => ({ ...current, prioridad: Number(event.target.value) as PrioridadTarea }))}
            >
              {Object.entries(prioridadLabels).map(([value, label]) => (
                <option key={value} value={value}>
                  {label}
                </option>
              ))}
            </select>
          </label>

          <label className="field">
            <span>Fecha de vencimiento</span>
            <input
              type="datetime-local"
              value={form.fechaVencimiento}
              onChange={(event) => setForm((current) => ({ ...current, fechaVencimiento: event.target.value }))}
            />
          </label>
        </div>

        <div className="three-column-form">
          <label className="field">
            <span>Usuario asignado</span>
            <select
              value={form.usuarioId}
              onChange={(event) => setForm((current) => ({ ...current, usuarioId: event.target.value }))}
            >
              <option value="">Sin asignar</option>
              {usuarios.map((item) => (
                <option key={item.id} value={item.id}>
                  {item.nombre}
                </option>
              ))}
            </select>
          </label>

          <label className="field">
            <span>Plantilla origen</span>
            <select
              value={form.plantillaTareaId}
              onChange={(event) => setForm((current) => ({ ...current, plantillaTareaId: event.target.value }))}
            >
              <option value="">Sin plantilla</option>
              {plantillas.map((item) => (
                <option key={item.id} value={item.id}>
                  {item.titulo}
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

        <div className="two-column-form">
          <label className="field field-inline">
            <input
              type="checkbox"
              checked={form.estaCompletada}
              onChange={(event) => setForm((current) => ({ ...current, estaCompletada: event.target.checked }))}
            />
            <span>Ya completada</span>
          </label>

          <label className="field field-inline">
            <input
              type="checkbox"
              checked={form.esRepetitiva}
              onChange={(event) =>
                setForm((current) => ({
                  ...current,
                  esRepetitiva: event.target.checked,
                  tipoRecurrencia: event.target.checked ? current.tipoRecurrencia || 1 : '',
                  proximaRecurrencia: event.target.checked ? current.proximaRecurrencia : ''
                }))
              }
            />
            <span>Es repetitiva</span>
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
            <span>Proxima recurrencia</span>
            <input
              type="datetime-local"
              value={form.proximaRecurrencia}
              onChange={(event) => setForm((current) => ({ ...current, proximaRecurrencia: event.target.value }))}
              disabled={!form.esRepetitiva}
            />
          </label>
        </div>

        <div className="form-actions">
          <button className="primary-button" type="submit" disabled={isSaving || !typeOptionsReady}>
            {editingId === null ? 'Crear' : 'Guardar'}
          </button>
          {editingId !== null ? (
            <button className="secondary-button" type="button" onClick={resetForm}>
              Cancelar edicion
            </button>
          ) : null}
        </div>

        {!typeOptionsReady ? (
          <p className="notice info">Necesitas al menos un tipo de tarea para crear nuevas tareas.</p>
        ) : null}
      </form>

      {feedback ? <p className="notice success">{feedback}</p> : null}
      {error ? <p className="notice error">{error}</p> : null}

      <div className="list-block">
        <div className="list-header">
          <h3>Tareas registradas</h3>
          <span>{tasks.length}</span>
        </div>

        {isLoading ? <p className="empty-state">Cargando...</p> : null}
        {!isLoading && tasks.length === 0 ? <p className="empty-state">No hay tareas todavia.</p> : null}

        <div className="entity-grid">
          {tasks.map((task) => (
            <article className="entity-card" key={task.id}>
              <div className="entity-title-row">
                <div>
                  <strong className={task.estaCompletada ? 'line-through' : ''}>{task.titulo}</strong>
                  <p>ID {task.id} · {task.tipoTareaNombre}</p>
                </div>
                <span className={task.estaCompletada ? 'status-pill status-ok' : 'status-pill'}>
                  {task.estaCompletada ? 'Completada' : 'Pendiente'}
                </span>
              </div>

              <dl className="details-list">
                <div>
                  <dt>Prioridad</dt>
                  <dd>{prioridadLabels[task.prioridad] ?? 'Desconocida'}</dd>
                </div>
                <div>
                  <dt>Creacion</dt>
                  <dd>{new Date(task.fechaCreacion).toLocaleString('es-ES')}</dd>
                </div>
                <div>
                  <dt>Vencimiento</dt>
                  <dd>{task.fechaVencimiento ? new Date(task.fechaVencimiento).toLocaleString('es-ES') : 'Sin fecha'}</dd>
                </div>
                <div>
                  <dt>Usuario</dt>
                  <dd>{task.usuarioNombre ?? 'Sin asignar'}</dd>
                </div>
              </dl>

              <div className="chip-row">
                {task.esRepetitiva ? (
                  <span className="status-pill">{recurrenciaLabels[task.tipoRecurrencia ?? 1] ?? 'Repetitiva'}</span>
                ) : (
                  <span className="status-pill">No repetitiva</span>
                )}
                {task.plantillaTareaId !== null ? <span className="status-pill">Plantilla {task.plantillaTareaId}</span> : null}
                {task.proximaRecurrencia ? (
                  <span className="status-pill">Siguiente {new Date(task.proximaRecurrencia).toLocaleString('es-ES')}</span>
                ) : null}
              </div>

              {task.notas ? <p className="muted-text">{task.notas}</p> : null}

              <div className="row-actions">
                <button type="button" className="primary-button" onClick={() => void handleComplete(task.id)} disabled={isSaving || task.estaCompletada}>
                  Completar
                </button>
                <button type="button" className="secondary-button" onClick={() => startEdit(task)}>
                  Editar
                </button>
                <button type="button" className="danger-button" onClick={() => void handleDelete(task.id)} disabled={isSaving}>
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