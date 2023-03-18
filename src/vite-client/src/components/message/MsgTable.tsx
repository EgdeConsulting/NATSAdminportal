import { PaginatedTable, MsgViewButton } from "components";

function MsgTable(props: { messages: any[] }) {
  const columns = [
    {
      Header: "MsgTable",
      columns: [
        {
          Header: "Sequence Number",
          accessor: "sequenceNumber",
          appendChildren: "false",
          rowBound: "false",
        },
        {
          Header: "Timestamp",
          accessor: "timestamp",
          appendChildren: "false",
          rowBound: "false",
        },
        {
          Header: "Stream",
          accessor: "stream",
          appendChildren: "false",
          rowBound: "false",
        },
        {
          Header: "Subject",
          accessor: "subject",
          appendChildren: "false",
          rowBound: "false",
        },
        {
          Header: "Data",
          accessor: "data",
          appendChildren: "true",
          rowBound: "true",
        },
      ],
    },
  ];

  return (
    <PaginatedTable columns={columns} data={props.messages}>
      <MsgViewButton content={""} />
    </PaginatedTable>
  );
}

export { MsgTable };
