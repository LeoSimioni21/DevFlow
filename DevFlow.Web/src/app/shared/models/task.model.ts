export type TaskStatus = 'analise' | 'em-desenvolvimento' | 'concluido';

export interface Task {
  id: string;
  projectId: string;
  title: string;
  description: string;
  assigneeInitials: string;
  status: TaskStatus;
  createdAt: string;
  updatedAt: string;
}
