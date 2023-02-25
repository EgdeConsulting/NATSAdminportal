import { PaginatedTable, MsgModal } from "components";

function MsgView(props: { messages: any[] }) {
  const columns = [
    {
      Header: "All Messages",
      columns: [
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
          Header: "Stream",
          accessor: "stream",
          appendChildren: "false",
        },
        {
          Header: "Subject",
          accessor: "subject",
          appendChildren: "false",
        },
        {
          Header: "Data",
          accessor: "data",
          appendChildren: "true",
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
