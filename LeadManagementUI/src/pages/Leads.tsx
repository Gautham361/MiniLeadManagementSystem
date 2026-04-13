import { useEffect, useState, useRef } from "react";
import api from "../api/axios";
import "../App.css";

export default function Leads() {
  const [leads, setLeads] = useState<any[]>([]);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("");

  const [showForm, setShowForm] = useState(false);
  const [selected, setSelected] = useState<any>(null);

  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  const [message, setMessage] = useState("");

  const [form, setForm] = useState({
    leadName: "",
    companyName: "",
    email: "",
    phoneNumber: "",
    assignedTo: "",
    status: "",
  });

  const [formErrors, setFormErrors] = useState<any>({});
  const [files, setFiles] = useState<File[]>([]);
  const [attachments, setAttachments] = useState<any[]>([]);

  const fileRef = useRef<HTMLInputElement | null>(null);

  useEffect(() => {
    fetchLeads();
  }, [page, search, statusFilter]);

  const fetchLeads = async () => {
    let query = `page=${page}`;
    if (search) query += `&search=${search}`;
    if (statusFilter) query += `&status=${statusFilter}`;

    const res = await api.get(`/leads?${query}`);
    setLeads(res.data.data.items);
    setTotalPages(res.data.data.totalPages);
  };

  const showMessage = (msg: string) => {
    setMessage(msg);
    setTimeout(() => setMessage(""), 3000);
  };

  const validateForm = () => {
    let err: any = {};

    if (!form.leadName) err.leadName = "Name is required";

    if (!form.companyName) err.companyName = "Company is required";

    if (!form.email) {
      err.email = "Email is required";
    } else if (!/\S+@\S+\.\S+/.test(form.email)) {
      err.email = "Invalid email format";
    }

    if (!form.phoneNumber) {
      err.phoneNumber = "Phone is required";
    } else if (form.phoneNumber.length > 10) {
      err.phoneNumber = "Max 10 digits allowed";
    } else if (!/^[0-9]{7,10}$/.test(form.phoneNumber)) {
      err.phoneNumber = "Invalid phone number";
    }

    if (!form.status) err.status = "Status is required";

    return err;
  };

  const reset = () => {
    setForm({
      leadName: "",
      companyName: "",
      email: "",
      phoneNumber: "",
      assignedTo: "",
      status: "",
    });

    setFiles([]);
    setAttachments([]);
    setSelected(null);
    setFormErrors({});
    setShowForm(false);

    if (fileRef.current) fileRef.current.value = "";
  };

  const handleCreate = () => {
    reset();
    setShowForm(true);
  };

  const handleSubmit = async () => {
    const errors = validateForm();

    if (Object.keys(errors).length > 0) {
      setFormErrors(errors);
      return;
    }

    setFormErrors({});

    try {
      let leadId;

      if (selected) {
        leadId = selected.leadId;
        await api.put(`/leads/${leadId}`, form);
        showMessage("Lead updated successfully");
      } else {
        const userId = localStorage.getItem("userId");

        const payload = {
          ...form,
          userId: parseInt(userId || "0"),
        };

        const res = await api.post(`/leads`, payload);
        leadId = res.data.data.leadId;
        showMessage("Lead created successfully");
      }

      if (files.length > 0) {
        const fd = new FormData();
        files.forEach((f) => fd.append("files", f));
        await api.post(`/leads/${leadId}/attachments`, fd);
      }

      await fetchLeads();
      reset();
    } catch (err: any) {
      showMessage(err.customMessage || "Operation failed");
    }
  };

  const selectLead = async (l: any) => {
    const res = await api.get(`/leads/${l.leadId}`);
    const data = res.data.data;

    setSelected(data);
    setShowForm(true);

    setForm({
      leadName: data.leadName || "",
      companyName: data.companyName || "",
      email: data.email || "",
      phoneNumber: data.phoneNumber || "",
      assignedTo: data.assignedTo || "",
      status: data.status || "",
    });

    setAttachments(data.attachments || []);

    if (fileRef.current) fileRef.current.value = "";
  };

  const deleteLead = async (id: number) => {
    if (!confirm("Delete lead?")) return;
    await api.delete(`/leads/${id}`);
    showMessage("Lead deleted successfully");
    fetchLeads();
  };

  const removeAttachment = async (id: number) => {
    await api.delete(`/leads/attachments/${id}`);
    setAttachments((prev) => prev.filter((a) => a.id !== id));
    showMessage("Attachment removed");
  };

  const downloadAttachment = async (id: number, name: string) => {
    const res = await api.get(`/leads/attachments/${id}/download`, {
      responseType: "blob",
    });

    const url = window.URL.createObjectURL(new Blob([res.data]));
    const link = document.createElement("a");
    link.href = url;
    link.setAttribute("download", name);
    link.click();
  };

  return (
    <>
      <div className="header">
        <h2>Lead Management System</h2>
        <button
          className="logout"
          onClick={() => {
            localStorage.clear();
            window.location.href = "/";
          }}
        >
          Logout
        </button>
      </div>

      <div className="container">
        {message && <div className="success-msg">{message}</div>}

        <div className="filter-bar">
          <div className="filter-left">
            <input
              placeholder="Search"
              value={search}
              onChange={(e) => {
                setPage(1);
                setSearch(e.target.value);
              }}
            />

            <select
              value={statusFilter}
              onChange={(e) => {
                setPage(1);
                setStatusFilter(e.target.value);
              }}
            >
              <option value="">Status</option>
              <option value="New">New</option>
              <option value="InProgress">InProgress</option>
              <option value="Converted">Converted</option>
              <option value="Lost">Lost</option>
            </select>
          </div>

          <button className="primary" onClick={handleCreate}>
            Create
          </button>
        </div>

        {showForm && (
          <div className="card">
            <h3>{selected ? "Edit Lead" : "Create Lead"}</h3>

            <div className="form-grid small">
              <div>
                <input
                  value={form.leadName}
                  placeholder="Name"
                  maxLength={100}
                  onChange={(e) =>
                    setForm({ ...form, leadName: e.target.value })
                  }
                />
                {formErrors.leadName && (
                  <div className="error-text">{formErrors.leadName}</div>
                )}
              </div>

              <div>
                <input
                  value={form.companyName}
                  placeholder="Company"
                  maxLength={100}
                  onChange={(e) =>
                    setForm({ ...form, companyName: e.target.value })
                  }
                />
                {formErrors.companyName && (
                  <div className="error-text">{formErrors.companyName}</div>
                )}
              </div>

              <div>
                <input
                  value={form.email}
                  placeholder="Email"
                  maxLength={100}
                  onChange={(e) =>
                    setForm({ ...form, email: e.target.value })
                  }
                />
                {formErrors.email && (
                  <div className="error-text">{formErrors.email}</div>
                )}
              </div>

              <div>
                <input
                  value={form.phoneNumber}
                  placeholder="Phone"
                  maxLength={20}
                  onChange={(e) => {
                    const value = e.target.value;
                    if (!/^[0-9]*$/.test(value)) return;
                    setForm({ ...form, phoneNumber: value });
                  }}
                />
                {formErrors.phoneNumber && (
                  <div className="error-text">{formErrors.phoneNumber}</div>
                )}
              </div>

              <div>
                <input
                  value={form.assignedTo}
                  placeholder="Assigned To"
                  maxLength={100}
                  onChange={(e) =>
                    setForm({ ...form, assignedTo: e.target.value })
                  }
                />
              </div>

              <div>
                <select
                  value={form.status}
                  onChange={(e) =>
                    setForm({ ...form, status: e.target.value })
                  }
                >
                  <option value="" disabled hidden>
                    Select Status
                  </option>
                  <option value="New">New</option>
                  <option value="InProgress">InProgress</option>
                  <option value="Converted">Converted</option>
                  <option value="Lost">Lost</option>
                </select>

                {formErrors.status && (
                  <div className="error-text">{formErrors.status}</div>
                )}
              </div>
            </div>

            <div className="file-row">
              <input
                ref={fileRef}
                type="file"
                multiple
                onChange={(e: any) =>
                  setFiles(Array.from(e.target.files))
                }
              />
            </div>

            {selected && attachments.length > 0 && (
              <div className="attachment-list">
                {attachments.map((a) => (
                  <div className="attachment" key={a.id}>
                    <span>{a.originalFileName}</span>

                    <div className="attachment-actions">
                      <button
                        className="primary"
                        onClick={() =>
                          downloadAttachment(a.id, a.originalFileName)
                        }
                      >
                        Download
                      </button>
                      <button onClick={() => removeAttachment(a.id)}>
                        Delete
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            )}

            <div className="form-actions">
              <button className="primary" onClick={handleSubmit}>
                {selected ? "Update" : "Create"}
              </button>
              <button onClick={reset}>Cancel</button>
            </div>
          </div>
        )}

        <div className="table-box">
          <h3>Lead List</h3>

          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Company</th>
                <th>Email</th>
                <th>Phone</th>
                <th>Status</th>
                <th>Attachments</th>
                <th>Actions</th>
              </tr>
            </thead>

            <tbody>
              {leads.map((l) => (
                <tr key={l.leadId} onClick={() => selectLead(l)}>
                  <td>{l.leadId}</td>
                  <td>{l.leadName}</td>
                  <td>{l.companyName}</td>
                  <td>{l.email}</td>
                  <td>{l.phoneNumber}</td>
                  <td>{l.status}</td>
                  <td>{l.attachments?.length || 0}</td>

                  <td>
                    <button
                      onClick={(e) => {
                        e.stopPropagation();
                        deleteLead(l.leadId);
                      }}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          <div className="pagination">
            <button disabled={page === 1} onClick={() => setPage(page - 1)}>Prev</button>
            <span>Page {page} of {totalPages}</span>
            <button disabled={page === totalPages} onClick={() => setPage(page + 1)}>Next</button>
          </div>
        </div>
      </div>
    </>
  );
}