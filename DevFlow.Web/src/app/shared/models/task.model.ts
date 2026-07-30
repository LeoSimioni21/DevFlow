export type TaskStatus = 'analise' | 'em-desenvolvimento' | 'concluido';
export type TaskPriority = 'baixa' | 'media' | 'alta';

export interface Task {
  id: string;
  projectId: string;
  code: string;
  title: string;
  description: string;
  assigneeInitials: string;
  status: TaskStatus;
  priority: TaskPriority;
  startedAt: string | null;
  finishedAt: string | null;
  hoursWorked: number | null;
  createdAt: string;
  updatedAt: string;
}
