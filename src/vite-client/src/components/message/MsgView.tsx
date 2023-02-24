import { PaginatedTable, MsgModal } from "components";

function MsgView(props: { messages: any[] }) {
  const columns = [
    {
      Header: "All Messages",
      columns: [
        {
          Header: "Stream",
          accessor: "stream",
          appendChildren: "false",
        },
        {
          Header: "Sequence Number",
          accessor: "sequenceNumber",
          appendChildren: "false",
        },
        {
          Header: "Timestamp",
          accessor: "timestamp",
          appendChildren: "false",
        },
        {
          Header: "Subject",
          accessor: "subject",
          appendChildren: "false",
        },
        {
          Header: "Acknowledgement",
          accessor: "acknowledgement",
          appendChildren: "false",
        },
      ],
    },
  ];

  return (
    <PaginatedTable columns={columns} data={props.messages}>
      <MsgModal></MsgModal>
    </PaginatedTable>
  );
}

export { MsgView };
