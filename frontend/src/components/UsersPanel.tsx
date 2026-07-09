import { useEffect, useMemo, useState } from 'react';
import { formatApiError } from '../api/client';
import {
  createUsuario,
  deleteUsuario,
  getDepartamentos,
  getPoblaciones,
  getSedes,
  getUsuarios,
  updateUsuario
} from '../api/resources';
import type { DepartamentoDto, PoblacionDto, SedeDto, UsuarioDto } from '../api/contracts';

interface UsuarioFormState {
  nombre: string;
  email: string;
  departamentoId: string;
  sedeId: string;
  poblacionId: string;
}

const initialFormState: UsuarioFormState = {
  nombre: '',
  email: '',
  departamentoId: '',
  sedeId: '',
  poblacionId: ''
};

export function UsersPanel() {
  const [users, setUsers] = useState<UsuarioDto[]>([]);
  const [departamentos, setDepartamentos] = useState<DepartamentoDto[]>([]);
  const [sedes, setSedes] = useState<SedeDto[]>([]);
  const [poblaciones, setPoblaciones] = useState<PoblacionDto[]>([]);
  const [form, setForm] = useState<UsuarioFormState>(initialFormState);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [feedback, setFeedback] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  async function reload() {
    setIsLoading(true);
    setError(null);

    try {
      const [loadedUsers, loadedDepartamentos, loadedSedes, loadedPoblaciones] = await Promise.all([
        getUsuarios(),
        getDepartamentos(),
        getSedes(),
        getPoblaciones()
      ]);

      setUsers(loadedUsers);
      setDepartamentos(loadedDepartamentos);
      setSedes(loadedSedes);
      setPoblaciones(loadedPoblaciones);
    } catch (caughtError) {
      setError(formatApiError(caughtError));
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    void reload();
  }, []);

  const hasReferenceData = useMemo(
    () => departamentos.length > 0 && sedes.length > 0 && poblaciones.length > 0,
    [departamentos.length, sedes.length, poblaciones.length]
  );

  function startEdit(item: UsuarioDto) {
    setEditingId(item.id);
    setForm({
      nombre: item.nombre,
      email: item.email ?? '',
      departamentoId: String(item.departamentoId),
      sedeId: String(item.sedeId),
      poblacionId: String(item.poblacionId)
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
        email: form.email.trim().length > 0 ? form.email.trim() : null,
        departamentoId: Number(form.departamentoId),
        sedeId: Number(form.sedeId),
        poblacionId: Number(form.poblacionId)
      };

      const savedUser = editingId === null
        ? await createUsuario(payload)
        : await updateUsuario(editingId, payload);

      setUsers((currentUsers) =>
        editingId === null
          ? [savedUser, ...currentUsers]
          : currentUsers.map((item) => (item.id === savedUser.id ? savedUser : item))
      );
      setFeedback(editingId === null ? 'Usuario creado.' : 'Usuario actualizado.');
      resetForm();
    } catch (caughtError) {
      setError(formatApiError(caughtError));
    } finally {
      setIsSaving(false);
    }
  }

  async function handleDelete(id: number) {
    if (!window.confirm('¿Eliminar este usuario?')) {
      return;
    }

    setIsSaving(true);
    setError(null);
    setFeedback(null);

    try {
      await deleteUsuario(id);
      setUsers((currentUsers) => currentUsers.filter((item) => item.id !== id));
      setFeedback('Usuario eliminado.');
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
          <p className="panel-kicker">Personas y organizacion</p>
          <h2>Usuarios</h2>
          <p className="panel-description">Gestion simple de usuarios enlazados a departamento, sede y poblacion.</p>
        </div>
      </div>

      <form className="stack-form" onSubmit={handleSubmit}>
        <label className="field">
          <span>Nombre</span>
          <input
            value={form.nombre}
            onChange={(event) => setForm((current) => ({ ...current, nombre: event.target.value }))}
            placeholder="Nombre completo"
            required
            maxLength={150}
          />
        </label>

        <label className="field">
          <span>Email opcional</span>
          <input
            type="email"
            value={form.email}
            onChange={(event) => setForm((current) => ({ ...current, email: event.target.value }))}
            placeholder="persona@example.com"
            maxLength={200}
          />
        </label>

        <div className="three-column-form">
          <label className="field">
            <span>Departamento</span>
            <select
              value={form.departamentoId}
              onChange={(event) => setForm((current) => ({ ...current, departamentoId: event.target.value }))}
              required
            >
              <option value="">Selecciona...</option>
              {departamentos.map((item) => (
                <option key={item.id} value={item.id}>
                  {item.nombre}
                </option>
              ))}
            </select>
          </label>

          <label className="field">
            <span>Sede</span>
            <select
              value={form.sedeId}
              onChange={(event) => setForm((current) => ({ ...current, sedeId: event.target.value }))}
              required
            >
              <option value="">Selecciona...</option>
              {sedes.map((item) => (
                <option key={item.id} value={item.id}>
                  {item.nombre}
                </option>
              ))}
            </select>
          </label>

          <label className="field">
            <span>Poblacion</span>
            <select
              value={form.poblacionId}
              onChange={(event) => setForm((current) => ({ ...current, poblacionId: event.target.value }))}
              required
            >
              <option value="">Selecciona...</option>
              {poblaciones.map((item) => (
                <option key={item.id} value={item.id}>
                  {item.nombre} ({item.codigoIsoPais})
                </option>
              ))}
            </select>
          </label>
        </div>

        <div className="form-actions">
          <button className="primary-button" type="submit" disabled={isSaving || !hasReferenceData}>
            {editingId === null ? 'Crear' : 'Guardar'}
          </button>
          {editingId !== null ? (
            <button className="secondary-button" type="button" onClick={resetForm}>
              Cancelar edicion
            </button>
          ) : null}
        </div>

        {!hasReferenceData ? (
          <p className="notice info">
            Para crear usuarios necesitas al menos un departamento, una sede y una poblacion.
          </p>
        ) : null}
      </form>

      {feedback ? <p className="notice success">{feedback}</p> : null}
      {error ? <p className="notice error">{error}</p> : null}

      <div className="list-block">
        <div className="list-header">
          <h3>Usuarios registrados</h3>
          <span>{users.length}</span>
        </div>

        {isLoading ? <p className="empty-state">Cargando...</p> : null}
        {!isLoading && users.length === 0 ? <p className="empty-state">No hay usuarios todavia.</p> : null}

        <div className="entity-grid">
          {users.map((item) => (
            <article className="entity-card" key={item.id}>
              <div className="entity-title-row">
                <div>
                  <strong>{item.nombre}</strong>
                  <p>ID {item.id}</p>
                </div>
                <span className="status-pill">Usuario</span>
              </div>

              <dl className="details-list">
                <div>
                  <dt>Email</dt>
                  <dd>{item.email ?? 'Sin email'}</dd>
                </div>
                <div>
                  <dt>Departamento</dt>
                  <dd>{item.departamentoNombre}</dd>
                </div>
                <div>
                  <dt>Sede</dt>
                  <dd>{item.sedeNombre}</dd>
                </div>
                <div>
                  <dt>Poblacion</dt>
                  <dd>{item.poblacionNombre} ({item.poblacionCodigoIsoPais})</dd>
                </div>
              </dl>

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