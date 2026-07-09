import { useState } from 'react';
import { TasksPanel } from './components/TasksPanel';
import { TemplatesPanel } from './components/TemplatesPanel';
import { UsersPanel } from './components/UsersPanel';
import { TextCatalogPanel } from './components/TextCatalogPanel';
import { PoblacionesPanel } from './components/PoblacionesPanel';
import { TiposTareaPanel } from './components/TiposTareaPanel';
import {
  createDepartamento,
  createSede,
  deleteDepartamento,
  deleteSede,
  getDepartamentos,
  getSedes,
  updateDepartamento,
  updateSede
} from './api/resources';

type TabKey = 'tareas' | 'plantillas' | 'usuarios' | 'departamentos' | 'sedes' | 'poblaciones' | 'tipos';

const tabs: Array<{ key: TabKey; label: string }> = [
  { key: 'tareas', label: 'Tareas' },
  { key: 'plantillas', label: 'Plantillas' },
  { key: 'usuarios', label: 'Usuarios' },
  { key: 'departamentos', label: 'Departamentos' },
  { key: 'sedes', label: 'Sedes' },
  { key: 'poblaciones', label: 'Poblaciones' },
  { key: 'tipos', label: 'Tipos de tarea' }
];

export function App() {
  const [activeTab, setActiveTab] = useState<TabKey>('tareas');

  return (
    <div className="app-shell">
      <header className="hero-banner">
        <div>
          <p className="eyebrow">DemoCopilot · frontend React + TypeScript + Vite</p>
          <h1>Gestion operativa de tareas con una interfaz directa y util</h1>
          <p className="hero-copy">
            El frontend consume la API local por proxy en desarrollo y concentra las llamadas HTTP en una sola capa de servicios.
          </p>
        </div>

        <div className="hero-card">
          <p>Backend local</p>
          <strong>https://localhost:55145</strong>
          <span>Vite proxy activo sobre /api</span>
        </div>
      </header>

      <nav className="tab-bar" aria-label="Secciones principales">
        {tabs.map((tab) => (
          <button
            key={tab.key}
            type="button"
            className={activeTab === tab.key ? 'tab-button active' : 'tab-button'}
            onClick={() => setActiveTab(tab.key)}
            aria-pressed={activeTab === tab.key}
          >
            {tab.label}
          </button>
        ))}
      </nav>

      <main className="page-grid">
        {activeTab === 'tareas' ? <TasksPanel /> : null}
        {activeTab === 'plantillas' ? <TemplatesPanel /> : null}
        {activeTab === 'usuarios' ? <UsersPanel /> : null}
        {activeTab === 'departamentos' ? (
          <TextCatalogPanel
            title="Departamentos"
            description="Catalogo simple para organizar usuarios por area."
            emptyHint="No hay departamentos todavia."
            loadItems={getDepartamentos}
            createItem={(nombre) => createDepartamento({ nombre })}
            updateItem={(id, nombre) => updateDepartamento(id, { nombre })}
            deleteItem={deleteDepartamento}
          />
        ) : null}
        {activeTab === 'sedes' ? (
          <TextCatalogPanel
            title="Sedes"
            description="Ubicaciones disponibles para asignar a usuarios."
            emptyHint="No hay sedes todavia."
            loadItems={getSedes}
            createItem={(nombre) => createSede({ nombre })}
            updateItem={(id, nombre) => updateSede(id, { nombre })}
            deleteItem={deleteSede}
          />
        ) : null}
        {activeTab === 'poblaciones' ? (
          <PoblacionesPanel />
        ) : null}
        {activeTab === 'tipos' ? <TiposTareaPanel /> : null}
      </main>

      <footer className="page-footer">
        <p>
          El formulario de tareas requiere un tipo de tarea valido; si la base esta vacia, crealo primero en la pestaña de tipos.
        </p>
      </footer>
    </div>
  );
}