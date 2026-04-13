import api from "../api/axios";

export const getLeads = async () => {
  const res = await api.get("/leads");
  return res.data.data.items;
};

export const createLead = async (lead: any) => {
  const res = await api.post("/leads", lead);
  return res.data.data;
};

export const updateLead = async (id: number, lead: any) => {
  const res = await api.put(`/leads/${id}`, lead);
  return res.data.data;
};

export const deleteLead = async (id: number) => {
  await api.delete(`/leads/${id}`);
};